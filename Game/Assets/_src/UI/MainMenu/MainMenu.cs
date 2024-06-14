using Common.Core;
using Common.Core.Loading;

using Game.Core.Loading;
using Game.Model.Locations;

using Reflex.Attributes;

using UnityEngine.UIElements;

namespace Game.UI
{
    public class MainMenu : UIWidget
    {
        [Inject] private IContainer m_Container;
        [Inject] private ILoadingManager m_LoadingManager;
        [Inject] private LocationScenes m_LocationScenes;
        [Inject] private IUIManager m_UIManager;

        protected override void Bind()
        {
            RootVisualElement.Q<Button>("new")
                .RegisterCallback<ClickEvent>(_ => LoadLocationAsync());
        }

        private async void LoadLocationAsync()
        {
            m_LocationScenes.TryGetSceneLocation("default", out var sceneRef);
            await m_LoadingManager.Start(m_Container, new CommandLoadLocation(m_Container, sceneRef).AsItem());
            m_UIManager.Hide<MainMenu>();
        }

    }
}
