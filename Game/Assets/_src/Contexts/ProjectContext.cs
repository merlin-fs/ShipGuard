using Common.Core.Loading;
using Game.Core.Loading;
using Game.Core.Repositories;

using Reflex.Core;

using UnityEngine;

namespace Game.Core
{
    public class ProjectContext : MonoBehaviour, IInstaller
    {
        [SerializeField] private LoadingConfig loadingConfig;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(c => c.Construct<ObjectRepository>());
            LoadGame(containerBuilder);
        }

        private void LoadGame(ContainerBuilder containerBuilder)
        {
            ILoadingManager loadingManager = new LoadingManager(containerBuilder.Build(), loadingConfig.GetCommands());
            containerBuilder.AddSingleton(loadingManager, typeof(ILoadingManager));
            loadingManager.Start();
        }
    }
}