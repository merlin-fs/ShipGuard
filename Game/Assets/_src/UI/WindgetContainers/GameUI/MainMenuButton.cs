using Common.Core;
using Common.Core.Loading;
using Common.UI;

using Game.Core.Camera;
using Game.Core.Loading;

using Reflex.Attributes;

using UnityEngine.UIElements;

namespace Game.UI
{
    public class MainMenuButton : UIVisualElementWidget
    {
        [Inject] private IContainer m_Container;
        [Inject] private ILoadingManager m_LoadingManager;
        [Inject] private IUIManager m_UIManager;
        [Inject] private CameraController m_CameraController;
        
        private const string TAG = "openMainMenu";
        private EventCallback<ClickEvent> m_EventCallback;
        private Button m_Button;
        
        public override void Bind(Binder<VisualElement> binder)
        {
            binder.AddAlternativeShowElement(binder.Element.Q<Button>(TAG), buttonBinder =>
            {
                buttonBinder.Callback<EventCallback<ClickEvent>>(_ => CloseLocationAsync(),
                    (b, action) => b.Element.RegisterCallback(action),
                    (b, action) => b.Element.UnregisterCallback(action));
            });
        }

        private async void CloseLocationAsync()
        {
            var command = new CommandCloseLocation(m_Container);
            await m_LoadingManager.Start(m_Container, command.AsItem());
            
            m_UIManager.Show<MainMenu>();
            m_UIManager.Hide<GameUI>();
            m_CameraController.SetActive(false);
        }
    }
}
