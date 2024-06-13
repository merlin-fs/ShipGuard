using Common.UI;

using Reflex.Attributes;

using UnityEngine.UIElements;

namespace Game.UI
{
    public abstract class UIWidget: IWidgetNoData
    {
        [Inject] private WidgetConfig m_WidgetConfig;
        protected VisualElement RootVisualElement { get; private set; }
        
        public void Bind(VisualElement rootVisualElement)
        {
            RootVisualElement = rootVisualElement;
            Bind();
        }
        protected abstract void Bind();
        
        public VisualTreeAsset GetTemplate()
        {
            m_WidgetConfig.TryGetTemplate(GetType(), out var template);
            return template;
        }
    }
}
