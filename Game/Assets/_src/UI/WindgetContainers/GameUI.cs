using Common.UI;

using UnityEngine.UIElements;

namespace Game.UI
{
    public class GameUI : UIWidgetContainer
    {
        public override void RegistrySubItems(RegistryContainer container)
        {
            container.Add<MainMenuButton>();
        }

        public override void Bind(Binder<VisualElement> binder)
        {
        }
    }
}
