using Common.UI;

using Reflex.Attributes;

using UnityEngine.UIElements;

namespace Game.UI
{
    public abstract class UIVisualElementWidget: IWidgetNoData<VisualElement>
    {
        [Inject] private WidgetConfig m_WidgetConfig;

        public abstract void Bind(Binder<VisualElement> binder);

        public VisualTreeAsset GetTemplate()
        {
            m_WidgetConfig.TryGetTemplate(GetType(), out var template);
            return template;
        }
    }
}
