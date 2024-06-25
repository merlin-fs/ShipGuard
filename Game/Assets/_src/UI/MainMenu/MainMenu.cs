using Common.Core;
using Common.Core.Loading;

using Game.Core.Camera;
using Game.Core.Loading;
using Game.Model.Locations;

using Reflex.Core;
using Reflex.Attributes;
using Reflex.Injectors;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
    public class MainMenu : UIWidget
    {
        [Inject] private IContainer m_Container;
        [Inject] private ILoadingManager m_LoadingManager;
        [Inject] private LocationScenes m_LocationScenes;
        [Inject] private IUIManager m_UIManager;
        [Inject] private CameraController m_CameraController;
        [Inject] private WidgetConfig m_WidgetConfig;

        protected override void Bind()
        {
            RootVisualElement.Q<Button>("new")
                .RegisterCallback<ClickEvent>(_ => LoadLocationAsync());
        }

        private async void LoadLocationAsync()
        {
            m_LocationScenes.TryGetSceneLocation("default", out var sceneRef);
            var command = new CommandLoadLocation(m_Container, sceneRef);
            await m_LoadingManager.Start(m_Container, command.AsItem());
            
            m_UIManager.Hide<MainMenu>();
            m_UIManager.Show<GameUI>()
                .WithConfig(m_WidgetConfig.GetTemplate<GameUI>());
            m_CameraController.SetActive(true);

            var test = new GameObject("_test_")
                .AddComponent<TestInitialization>()
                .AddComponent<test_MoveComponent>()
                .AddComponent<PlayerInputHandler>();
            GameObjectInjector.InjectObject(test.gameObject, (Container)command.GetContainer());
        }
    }
}
