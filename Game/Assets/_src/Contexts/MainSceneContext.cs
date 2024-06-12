using Game.Core.Camera;
using Game.Core.Inputs;
using Game.Core.Spawns;
using Game.Model.Locations;

using Reflex.Core;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game.Core.Contexts
{
    public class MainSceneContext : MonoBehaviour, IInstaller
    {
        [SerializeField] private InputActionAsset playerInputAsset;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private SpawnFactory spawnViewFactory;
        [SerializeField] private LocationScenes locationScenes;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddTransient(c => c.Construct<Spawner>(spawnViewFactory));
            containerBuilder.AddSingleton<IPlayerInputs>(c => c.Construct<PlayerInputs>(playerInputAsset));
            containerBuilder.AddSingleton<CameraController>(c => cameraController);

            containerBuilder.OnContainerBuilt += async container =>
            {
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