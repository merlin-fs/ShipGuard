using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Common.Core;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Common.UI
{
    public partial class UIManager<TLayer>: IUIManager<TLayer>
        where TLayer : struct, IComparable, IConvertible, IFormattable
    {
        private readonly IContainer m_Container;
        private readonly Dictionary<TLayer, UIDocument> m_Layers = new();
        private readonly ConcurrentQueue<WidgetShowInternalBinder> m_WidgetShowQueue = new();
        private readonly TLayer m_DefaultLayer;
        private readonly Dictionary<Type, CacheWidget> m_CacheWidget = new();

        private Task m_ProcessTask;
        private Action m_ResumeProcess;

        protected UIManager(IContainer container, [NotNull] GameObject host, TLayer defaultLayer, params (TLayer layer, PanelSettings panelSettings, VisualTreeAsset rootElement)[] layers)
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
                m_Layers.Add(layer, uiDocument);
                
                //m_Document.rootVisualElement?.RegisterCallback<NavigationCancelEvent>(evt => OnCancel());
            }
            //m_Document = rootDoc;
            //m_Document.enabled = true;
            //var cancelWidget = new RootClicked(m_Document.rootVisualElement);
            //cancelWidget.Bind(m_Document);
            //m_Cancel = cancelWidget.VisualElement;
            //ShowElement(m_Cancel, false);
            //m_Document.rootVisualElement?.RegisterCallback<NavigationCancelEvent>(evt => OnCancel());
            m_ProcessTask = Process();
        }

        
        public IUIManager<TLayer>.WidgetShowBinder Show<T>() 
            where T : IWidget
        {
            var binder = new WidgetShowInternalBinder(this, typeof(T), true);
            m_WidgetShowQueue.Enqueue(binder);
            ResumeProcess();
            return binder;
        }

        public void Hide<T>() 
            where T : IWidget
        {
            var binder = new WidgetShowInternalBinder(this, typeof(T), false);
            binder.WithLayer(m_DefaultLayer);
            m_WidgetShowQueue.Enqueue(binder);
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
                    while (m_WidgetShowQueue.TryDequeue(out var showBinder))
                    {
                        if (showBinder.Show)
                            UnityMainThread.Context.Post(target => Show((WidgetShowInternalBinder)target), showBinder);
                        else
                            UnityMainThread.Context.Post(target => Hide((WidgetShowInternalBinder)target), showBinder);
                    }
                }
            }, token);
        }

        private void Show(WidgetShowInternalBinder showBinder)
        {
            if (!m_CacheWidget.TryGetValue(showBinder.WidgetType, out var cacheWidget))
            {
                var widget = showBinder.Create();
                var doc = m_Layers[showBinder.Layer];
                var element = showBinder.AttachDocument(doc.rootVisualElement);
                cacheWidget = new CacheWidget {Widget = widget, Root = doc.rootVisualElement, Element = element};
                showBinder.Bind(cacheWidget.Root);
                m_CacheWidget.Add(showBinder.WidgetType, cacheWidget);
            }
            ShowElement(cacheWidget.Element, true);
        }

        private void Hide(WidgetShowInternalBinder showBinder)
        {
            if (!m_CacheWidget.TryGetValue(showBinder.WidgetType, out var cacheWidget)) return;
            ShowElement(cacheWidget.Element, false);
        }

        private void OnCancel()
        {
            
        }

        private static void ShowElement(VisualElement element, bool show)
        {
            if (element == null)
                return;
            var style = show
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            if (element.style.display != style)
            {
                using (var change = ChangeEvent<DisplayStyle>.GetPooled(element.style.display.value, style))
                {
                    change.target = element;
                    element.panel?.visualTree.SendEvent(change);
                }
                element.style.display = style;
            }
        }

        private IWidget Create(Type widgetType)
        {
            return (IWidget)m_Container.Instantiate(widgetType);
        }

        private class CacheWidget
        {
            public IWidget Widget;
            public VisualElement Root;
            public VisualElement Element;
        }
    }
}