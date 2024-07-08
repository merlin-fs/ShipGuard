using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Common.Core;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Common.UI
{
    public abstract class UIManager<TLayer, T, TManifest>: IUIManager<TLayer, T, TManifest>
        where TLayer : struct, IComparable, IConvertible, IFormattable
        where TManifest : WidgetShowManifest<TLayer, T>
    {
        private readonly IContainer m_Container;
        private readonly Dictionary<TLayer, Binder<T>> m_Layers = new();
        private readonly ConcurrentQueue<WidgetShowManifest<TLayer, T>.IAccessManifest> m_WidgetShowQueue = new();
        private readonly TLayer m_DefaultLayer;
        private readonly Dictionary<Type, CacheWidget> m_CacheWidget = new();

        private Task m_ProcessTask;
        private Action m_ResumeProcess;

        protected UIManager(IContainer container, [NotNull] GameObject host, 
            TLayer defaultLayer, params (TLayer layer, PanelSettings panelSettings, VisualTreeAsset rootElement)[] layers)
        {
            Assert.AreNotEqual(layers.Length, 0, $"UIManager requires at least one UIDocument");
            m_Container = container;
            m_DefaultLayer = defaultLayer;

            
            foreach ((TLayer layer, PanelSettings panelSettings, VisualTreeAsset rootElement) in layers)
            {
                var obj = new GameObject(layer.ToString(), typeof(UIDocument));
                obj.transform.SetParent(host.transform);
                var uiDocument = obj.GetComponent<UIDocument>();
                uiDocument.panelSettings = panelSettings;
                uiDocument.visualTreeAsset = rootElement;
                
                m_Layers.Add(layer, GetRootBinder(uiDocument));
            }
            m_ProcessTask = Process();
        }

        protected abstract Binder<T> GetRootBinder(object layerObject);

        protected Binder<T> GetBinder(T element)
        {
            return Binder<T>.GetNewBinder(element);
        }
        
        public TManifest Show<TWidget>() 
            where TWidget : IWidget
        {
            var manifestAccess = new WidgetShowManifest<TLayer, T>.AccessManifest<TManifest>(
                m_Container.Instantiate<TManifest>(typeof(TWidget), true), m_Container);
            
            manifestAccess.Manifest.WithLayer(m_DefaultLayer);
            m_WidgetShowQueue.Enqueue(manifestAccess);
            ResumeProcess();
            return manifestAccess.Manifest;
        }

        public void Hide<TWidget>() 
            where TWidget : IWidget
        {
            var manifestAccess = new WidgetShowManifest<TLayer, T>.AccessManifest<TManifest>(
                m_Container.Instantiate<TManifest>(typeof(TWidget), false), m_Container);
            manifestAccess.Manifest.WithLayer(m_DefaultLayer);
            m_WidgetShowQueue.Enqueue(manifestAccess);
            ResumeProcess();
        }

        private void ResumeProcess() => Task.Delay(1).ContinueWith(task => m_ResumeProcess.Invoke());
        
        private Task Process()
        {
            EventWaitHandle @event = new AutoResetEvent(false);
            CancellationToken token = new CancellationToken();
            m_ResumeProcess = () => @event.Set(); 
            
            return Task.Run(() =>
            {
                while (@event.WaitOne())
                {
                    while (m_WidgetShowQueue.TryDequeue(out var manifestAccess))
                    {
                        UnityMainThread.Context
                            .Post(target => ProcessingManifest(
                                (WidgetShowManifest<TLayer, T>.IAccessManifest)target), 
                                manifestAccess);   
                    }
                }
            }, token);
        }

        private void ProcessingManifest(WidgetShowManifest<TLayer, T>.IAccessManifest manifest)
        {
            var cacheWidget = GetCacheWidget(manifest);
            if (cacheWidget == null) return;
            if (manifest.IsShow)
                Show(cacheWidget);
            else
                Hide(cacheWidget);
        }
        
        private CacheWidget GetCacheWidget(WidgetShowManifest<TLayer, T>.IAccessManifest manifest)
        {
            if (m_CacheWidget.TryGetValue(manifest.WidgetType, out var cacheWidget) || !manifest.IsShow)
                return cacheWidget;

            var widget = manifest.CreateWidget();
            var binder = GetBinder(manifest.AttachDocument(widget, m_Layers[manifest.Layer]));
            var callbackHandlers = ListPool<CallbackHandler>.Get();

            if (widget is IWidgetContainer widgetContainer)
            {
                foreach (var widgetType in widgetContainer.GetWidgetTypes())
                {
                    var manifestAccess = new WidgetShowManifest<TLayer, T>.AccessManifest<TManifest>(
                        m_Container.Instantiate<TManifest>(widgetType, manifest.IsShow), m_Container);
                    manifestAccess.Manifest.WithLayer(manifest.Layer);

                    var cacheSubItem = GetCacheWidget(manifestAccess);
                    if (cacheSubItem == null) continue;
                    callbackHandlers.AddRange(cacheSubItem.CallbackHandlers);
                }
            }
            
            manifest.BindingObjectAction.Invoke(binder);

            var (elements, handlers) = binder.Build();
            callbackHandlers.AddRange(handlers);
            cacheWidget = new CacheWidget 
            {
                Widget = widget,
                Elements = elements,
                CallbackHandlers = callbackHandlers.ToArray(),
            };
            ListPool<CallbackHandler>.Release(callbackHandlers);
            binder.Release();
            
            m_CacheWidget.Add(manifest.WidgetType, cacheWidget);
            return cacheWidget;
        }

        private void RemoveCache(CacheWidget cacheWidget)
        {
            var removed = ListPool<Type>.Get();
            removed.Add(cacheWidget.Widget.GetType());
            
            
            if (cacheWidget.Widget is IWidgetContainer widgetContainer)
            {
                foreach (var widgetType in widgetContainer.GetWidgetTypes())
                {
                    var manifestAccess = new WidgetShowManifest<TLayer, T>.AccessManifest<TManifest>(
                        m_Container.Instantiate<TManifest>(widgetType, false), m_Container);

                    var cacheSubItem = GetCacheWidget(manifestAccess);
                    if (cacheSubItem == null) continue;
                    removed.Add(cacheSubItem.Widget.GetType());
                }
            }

            foreach (var iter in cacheWidget.CallbackHandlers)
            {
                iter.RemoveHandler();
            }
                
            foreach (var iter in removed)
            {
                m_CacheWidget.Remove(iter);
            }
        }
        
        private void Show(CacheWidget cacheWidget)
        {
            if (cacheWidget == null) return;
            foreach (var iter in cacheWidget.CallbackHandlers)
            {
                iter.AddHandler();
            }
            ShowElements(true, cacheWidget.Elements);
        }

        private void Hide(CacheWidget cacheWidget)
        {
            if (cacheWidget == null) return;
            ShowElements(false, cacheWidget.Elements);
            foreach (var iter in cacheWidget.CallbackHandlers)
            {
                iter.RemoveHandler();
            }
        }

        protected abstract void ShowElement(bool show, T element);
        
        private void ShowElements(bool show, IEnumerable<T> elements)
        {
            foreach (var element in elements)
            {
                if (element == null)
                    continue;

                ShowElement(show, element);
            }
        }

        private class CacheWidget
        {
            public IWidget Widget;
            public T[] Elements;
            public CallbackHandler[] CallbackHandlers;
        }
    }
}