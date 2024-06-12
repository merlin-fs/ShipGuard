using Common.Core.Loading;

using Game.Core.Loading;
using Game.Core.Repositories;

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
            containerBuilder.AddSingleton<ILoadingManager>(c => c.Construct<LoadingManager>(c, loadingConfig.GetCommands()));

            containerBuilder.OnContainerBuilt += container =>
            {
                container.Resolve<ILoadingManager>().Start();
            };
            /*
#if UNITY_EDITOR
            var scene = SceneManager.GetActiveScene();
            if (scene.IsValid() && scene.isLoaded)
            {
                scene.GetRootGameObjects()
                await SceneManager.LoadSceneAsync(0).ToUniTask();
            }
#endif
            */
        }
    }
}