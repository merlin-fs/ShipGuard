using Common.Core;
using Common.Core.Loading;
using Common.UI;

using Game.Core.Camera;
using Game.Core.Loading;

using Reflex.Attributes;

using UnityEngine.UIElements;

namespace Game.UI
{
    public class MainMenu : UIVisualElementWidget
    {
        [Inject] private IContainer m_Container;
        [Inject] private ILoadingManager m_LoadingManager;
        [Inject] private IUIManager m_UIManager;
        [Inject] private CameraController m_CameraController;
        [Inject] private WidgetConfig m_WidgetConfig;

        public override void Bind(Binder<VisualElement> binder)
        {
            var newButton = binder.Element.Q<Button>("new");
            binder.Callback<EventCallback<ClickEvent>>(
                _ => LoadLocationAsync(),
                (_, action) => newButton.RegisterCallback(action),
                (_, action) => newButton.UnregisterCallback(action));
        }

        private async void LoadLocationAsync()
        {
            var command = new CommandLoadLocation(m_Container, "default");
            await m_LoadingManager.Start(m_Container, command.AsItem());
            
            m_UIManager.Hide<MainMenu>();
            m_UIManager.Show<GameUI>()
                .WithConfig(m_WidgetConfig.GetTemplate<GameUI>())
                .WithLayer(UILayer.Main);
            
            m_CameraController.SetActive(true);
        }
    }
}
