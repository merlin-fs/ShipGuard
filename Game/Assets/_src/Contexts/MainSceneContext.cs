
using Common.Core.Loading;

using Game.Core.Camera;
using Game.Core.Inputs;
using Game.Core.Spawns;
using Game.Model.Locations;
using Game.UI;

using Reflex.Attributes;
using Reflex.Core;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Game.Core.Contexts
{
    public class MainSceneContext : MonoBehaviour, IInstaller
    {
        [SerializeField] private InputActionAsset playerInputAsset;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private SpawnFactory spawnViewFactory;
        [SerializeField] private LocationScenes locationScenes;
        [SerializeField] private GameObject uiManagerHost;
        
        [Inject] private WidgetConfig m_WidgetConfig;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddTransient(c => c.Construct<Spawner>(spawnViewFactory));
            containerBuilder.AddSingleton<IPlayerInputs>(c => c.Construct<PlayerInputs>(playerInputAsset));
            containerBuilder.AddSingleton<CameraController>(c => cameraController);

            containerBuilder.AddSingleton<IUIManager>(container =>
            {
                var manager = container.Construct<UIManager>(
                    container, 
                    uiManagerHost, 
                    UILayer.Main,
                    container.Resolve<WidgetConfig>().GetLayers);
                //UI widgets
                /*
                manager.WithBindWidget(binder =>
                {
                    //binder.Bind<GameUI>();
                    //binder.Bind<LeftPanel>();
                });
                */
                return manager;
            });

            containerBuilder.OnContainerBuilt += async container =>
            {
                container.Resolve<IUIManager>().Show<UI.Loading>()
                    .WithData(() => container.Resolve<ILoadingManager>().Progress);
                
                foreach (var sceneRef in locationScenes.Scenes)
                {
                    var loc = await Addressables.LoadResourceLocationsAsync(sceneRef).Task;
                    var id = Addressables.ResourceManager.TransformInternalId(loc[0]);
                    var scene = SceneManager.GetSceneByPath(id);
                    ReflexSceneManager.OverrideSceneParentContainer(scene: scene, parent: container);
                }
            };
        }
    }
}