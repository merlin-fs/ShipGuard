using Game.Core.Inputs;

using Reflex.Core;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Core.Contexts
{
    public class MainSceneContext : MonoBehaviour, IInstaller
    {
        [SerializeField] private InputActionAsset playerInputAsset;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton<IPlayerInputs>(c => c.Construct<PlayerInputs>(playerInputAsset));
        }
    }
}