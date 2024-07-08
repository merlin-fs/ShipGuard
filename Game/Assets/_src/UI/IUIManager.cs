using System;

using Common.Core;
using Common.UI;

using UnityEngine.UIElements;

namespace Game.UI
{
    public enum UILayer
    {
        Loading,
        Windows,
        Main,
    }

    public interface IUIManager : IUIManager<UILayer, VisualElement, WidgetShowManifest>
    {
    }
    
    public class WidgetShowManifest : WidgetShowManifest<UILayer, VisualElement>
    {
        public WidgetShowManifest(Type widgetType, bool show) : base(widgetType, show)
        {
        }

        protected VisualTreeAsset VisualTreeAsset { get; set;}

        public WidgetShowManifest WithConfig(VisualTreeAsset asset)
        {
            VisualTreeAsset = asset;
            return this;
        }
        
        protected override IWidget CreateWidget(IContainer container)
        {
            return (IWidget)container.Instantiate(WidgetType);
        }

        protected override VisualElement AttachDocument(IWidget widget, Binder<VisualElement> parent)
        {
            VisualTreeAsset ??= widget.GetTemplate();
            if (VisualTreeAsset == null) return parent.Element;
            
            VisualTreeAsset.CloneTree(parent.Element, out var index, out var count);
            if (count <= 0)
                throw new Exception($"No insert VisualTreeAsset {VisualTreeAsset}");
            return parent.Element[index];
        }
    }
}
