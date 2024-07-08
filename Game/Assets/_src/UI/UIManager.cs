using System;

using Common.Core;
using Common.UI;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
    public class UIManager : UIManager<UILayer, VisualElement, WidgetShowManifest>, IUIManager
    {
        public UIManager(IContainer container, [NotNull] GameObject host, 
            UILayer defaultLayer, 
            params (UILayer layer, PanelSettings panelSettings, VisualTreeAsset rootElement)[] layers) : base(container, host, defaultLayer, layers)
        {
        }

        protected override void ShowElement(bool show, VisualElement element)
        {
            var style = show
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            if (element.style.display == style) return;
                
            using (var change = ChangeEvent<DisplayStyle>.GetPooled(element.style.display.value, style))
            {
                change.target = element;
                element.panel?.visualTree.SendEvent(change);
            }
            element.style.display = style;
        }

        protected override Binder<VisualElement> GetRootBinder(object layerObject)
        {
            return GetBinder(((UIDocument)layerObject).rootVisualElement);
        }

        private class DisposeCallback : IDisposable
        {
            private readonly Action m_Dispose;

            public DisposeCallback(Action actionDispose)
            {
                m_Dispose = actionDispose;
            }
            
            public void Dispose()
            {
                m_Dispose.Invoke();
            }
        }
    }
}
