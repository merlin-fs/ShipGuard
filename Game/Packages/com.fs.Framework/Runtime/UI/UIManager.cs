using System;
using System.Linq;
using System.Collections.Generic;
using Common.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Common.UI
{
    public class UIManager: IUIManager, IInitiated
    {
        private IContainer m_Container;
        private readonly UIDocument m_Document;
        private readonly VisualElement m_Cancel;
        private readonly List<VisualElement> m_PopupStack = new();
        private readonly Dictionary<Type, IWidget> m_Parts = new();

        private int m_CancelRef;
        private Initialization m_Initialization;
        private IInitiated _initiatedImplementation;

        #region Initialize
        public void Initialize(IContainer container)
        {
            m_Document.enabled = true;
            if (m_Initialization == null) return;

            if (m_Initialization.BindWidget != null)
            {
                var binder = new WriterWidgetBinder();
                m_Initialization.BindWidget.Invoke(binder);
                InitializeWidgets(binder.Build());
            }
            m_Initialization = null;
        }
        #endregion
        private class Initialization
        {
            public Action<WidgetBinder> BindWidget;
        }
        
        public UIManager(GameObject root, string rootName)
        {
            m_Document = root.GetComponent<UIDocument>();
            m_Document.enabled = true;
            var cancelWidget = new RootClicked(rootName);
            cancelWidget.Bind(m_Document);
            m_Cancel = cancelWidget.VisualElement;
            ShowElement(m_Cancel, false);
            m_Document.rootVisualElement?.RegisterCallback<NavigationCancelEvent>(evt => OnCancel());
        }

        public void WithBindWidget(Action<WidgetBinder> bind)
        {
            m_Initialization ??= new Initialization();
            m_Initialization.BindWidget = bind;
        } 

        public void Show<T>(bool show)
            where T: IWidget
        {
            var widget = GetPart(typeof(T));
            ShowElements(widget, show);
        }

        private UIWidget GetPart(Type type)
        {
            return (UIWidget)m_Parts[type];
        }

        private void InitializeWidgets(IEnumerable<Type> widgets)
        {
            foreach (var widgetType in widgets)
            {
                var widget = AddWidged(widgetType);
                if (widget is IWidgetContainer container)
                    InitializeWidgets(container.GetWidgetTypes());
                ShowElements(widget, false);
            }

            UIWidget AddWidged(Type type)
            {
                if (m_Parts.TryGetValue(type, out var value)) return (UIWidget)value; 

                var widget = (UIWidget)m_Container.Instantiate(type);
                m_Parts.Add(type, widget);
                widget.Bind(m_Document);
                return widget;
            }
        }

        private void ShowElements(UIWidget widget, bool show)
        {
            foreach (var iter in widget.GetElements())
            {
                ShowElement(iter, show);
            }

            if (widget is not IWidgetContainer container) return;

            foreach (var iter in container.GetWidgetTypes())
            {
                ShowElements(GetPart(iter), show);
            }
        }
        
        public void Show(VisualElement element, ShowStyle style = ShowStyle.Normal)
        {
            switch (style)
            {
                case ShowStyle.Normal:
                    ShowElement(element, true);
                    return;
                case ShowStyle.Popup:
                    ShowPopups(element);
                    return;
            }
        }

        public void Close(VisualElement element)
        {
            m_PopupStack.Remove(element);
            ShowCancelButton(false);
            ShowElement(element, false);
        }

        public void HidePopups()
        {
            foreach (var iter in m_PopupStack)
            {
                ShowCancelButton(false);
                ShowElement(iter, false);
            }
        }

        public void RestorePopups()
        {
            ShowCancelButton(true);
            foreach (var iter in m_PopupStack)
                ShowElement(iter, true);
        }

        private void ShowPopups(VisualElement element)
        {
            ShowCancelButton(true);
            ShowElement(element, true);
            if (!m_PopupStack.Contains(element))
                m_PopupStack.Add(element);
        }

        private void ClosePopups()
        {
            while (m_PopupStack.Count > 0)
            {
                var item = m_PopupStack.Last();
                m_PopupStack.RemoveAt(m_PopupStack.Count - 1);
                ShowCancelButton(false);
                ShowElement(item, false);
            }
        }

        public void ShowCancelButton(bool show)
        {
            if (show)
                ShowElement(m_Cancel, ++m_CancelRef > 0);
            else
                ShowElement(m_Cancel, --m_CancelRef > 0);
            if (m_CancelRef < 0) m_CancelRef = 0;
        }

        private void OnCancel()
        {
            ClosePopups();
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

        private class WriterWidgetBinder : WidgetBinder
        {
            public IEnumerable<Type> Build() => m_WidgetTypes;
        }
    }
}