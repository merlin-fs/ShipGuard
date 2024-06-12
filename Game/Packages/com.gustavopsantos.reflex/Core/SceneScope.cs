using Reflex.Injectors;
using Reflex.Logging;

using UnityEngine;
using UnityEngine.Pool;

namespace Reflex.Core
{
    [DefaultExecutionOrder(int.MinValue)]
    public sealed class SceneScope : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR
            if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene() == gameObject.scene && 
                gameObject.scene.isLoaded && gameObject.scene.buildIndex < 0)
                return;
#endif
            UnityInjector.OnSceneLoaded.Invoke(gameObject.scene, this);
        }

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            using var pooledObject = ListPool<IInstaller>.Get(out var installers);
            GetComponentsInChildren<IInstaller>(installers);
            
            for (var i = 0; i < installers.Count; i++)
            {
                installers[i].InstallBindings(containerBuilder);
            }

            ReflexLogger.Log($"SceneScope ({gameObject.scene.name}) Bindings Installed", LogLevel.Info, gameObject);
        }
    }
}