using Game.Core.Camera;
using Game.Core.Inputs;
using Game.Core.Spawns;
using Game.Model.Locations;
using Game.Model.Units;
using Game.Storages;
using Game.UI;

using Reflex.Attributes;
using Reflex.Core;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core.Contexts
{
    public class MainSceneContext : MonoBehaviour, IInstaller
    {
        [SerializeField] private InputActionAsset playerInputAsset;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private SpawnFactory spawnViewFactory;
        [SerializeField] private LocationScenes locationScenes;
        [SerializeField] private GameObject uiManagerHost;
        [SerializeField] private PlayerInputHandler playerInputHandler;
        
        [Inject] private WidgetConfig m_WidgetConfig;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            
            containerBuilder.AddSingleton(playerInputHandler);
            containerBuilder.AddSingleton(c => c.Construct<LocationManager>(locationScenes));
            
            containerBuilder.AddSingleton(c =>
            {
                var storageManager = c.Construct<StorageManager>();
                storageManager.SetContainerEndpoint<StorageUser>(new LocalStorageUserEndpoint(Application.persistentDataPath));
                return storageManager;
            });
            
            containerBuilder.AddSingleton<Spawn.IViewFactory>(c => spawnViewFactory);

            containerBuilder.AddSingleton<IPlayerInputs>(c => c.Construct<PlayerInputs>(playerInputAsset));
            containerBuilder.AddSingleton<UnitManager>(c => c.Construct<UnitManager>());
            
            containerBuilder.AddSingleton<CameraController>(c =>
            {
                cameraController.SetActive(false);
                return cameraController;
            });

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
        }
    }
}