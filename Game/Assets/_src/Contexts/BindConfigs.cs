using Reflex.Core;

using UnityEngine;

namespace Game.Core.Contexts
{
    public class BindConfigs : MonoBehaviour, IInstaller
    {
        [SerializeField] private ScriptableBindConfig[] configs;
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            foreach (var iter in configs)
            {
                iter.Bind(containerBuilder);
            }
        }
    }
}
