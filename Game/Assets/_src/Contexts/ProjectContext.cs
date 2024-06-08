using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using Game.Core.Loading;
using Game.Core.Repositories;
using Game.Core.Spawns;

using Reflex.Core;

using UnityEngine;
using UnityEngine.SceneManagement;

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

        private async void LoadGame(ContainerBuilder containerBuilder)
        {
            /*
#if UNITY_EDITOR
            var scene = SceneManager.GetActiveScene();
            if (scene.IsValid() && scene.isLoaded)
            {
                await SceneManager.LoadSceneAsync(0).ToUniTask();
            }
#endif
*/
            ILoadingManager loadingManager = new LoadingManager(containerBuilder.Build(), loadingConfig.GetCommands());
            containerBuilder.AddSingleton(loadingManager, typeof(ILoadingManager));
            loadingManager.Start();
        }
    }
}