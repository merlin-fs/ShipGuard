using Common.Core.Loading;
using Game.Core.Loading;
using Game.Core.Repositories;
using Game.Core.Spawns;

using Reflex.Core;

using UnityEngine;

namespace Game.Core.Contexts
{
    public class ProjectContext : MonoBehaviour, IInstaller
    {
        [SerializeField] private LoadingConfig loadingConfig;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(c => c.Construct<ObjectRepository>());
            containerBuilder.AddTransient(c => c.Construct<Spawner>(null));//new Spawn.IViewFactory(){} 
            
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