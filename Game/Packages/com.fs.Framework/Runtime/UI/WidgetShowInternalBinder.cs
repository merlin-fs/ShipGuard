using System;
using UnityEngine.UIElements;

namespace Common.UI
{
    public partial class UIManager<TLayer>
    {
        private class WidgetShowInternalBinder : IUIManager<TLayer>.WidgetShowBinder
        {
            private readonly UIManager<TLayer> m_UIManager;
                
            public new Type WidgetType => base.WidgetType;
            public new TLayer Layer => base.Layer;
            public new bool Show => base.Show;
            
            public WidgetShowInternalBinder(UIManager<TLayer> uiManager, Type widgetType, bool show) : base(widgetType, show)
            {
                m_UIManager = uiManager;
            }

            public void Bind(VisualElement root)
            {
                BindingObject?.Invoke(root);
            }
            
            public VisualElement AttachDocument(VisualElement root)
            {
                VisualTreeAsset.CloneTree(root, out var index, out var count);
                if (count <= 0)
                    throw new Exception($"No insert VisualTreeAsset {VisualTreeAsset}");
                    
                return root[index];
            }
            
            public IWidget Create()
            {
                var widget = m_UIManager.Create(WidgetType);
                GetWidget = () => widget; 
                VisualTreeAsset ??= widget.GetTemplate();
                return widget;
            }
        }
    }
}
