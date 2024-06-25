using System.Linq;

using Common.Core.Loading;

using Game.Core.Loading;
using Game.Core.Repositories;

using Reflex.Core;

using UnityEngine;

namespace Game.Core.Contexts
{
    public class ProjectContext : MonoBehaviour, IInstaller
    {
        [SerializeField] private LoadingBindConfig loadingConfig;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(c => c.Construct<ConfigRepository>());
            containerBuilder.AddSingleton<ILoadingManager>(c => c.Construct<LoadingManager>());

            containerBuilder.OnContainerBuilt += container =>
            {
                container.Resolve<ILoadingManager>().Start(container, loadingConfig.GetCommands().ToArray());
            };
        }
    }
}