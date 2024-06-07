using Game.Core.Camera;
using Game.Core.Inputs;
using Game.Core.Spawns;

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
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddTransient(c => c.Construct<Spawner>(spawnViewFactory));
            
            containerBuilder.AddSingleton<IPlayerInputs>(c => c.Construct<PlayerInputs>(playerInputAsset));
            containerBuilder.AddSingleton<CameraController>(c => cameraController);
        }
    }
}