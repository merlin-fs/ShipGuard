using System;
using JetBrains.Annotations;
using Reflex.Injectors;
using UnityEngine.SceneManagement;

namespace Reflex.Core
{
    public static class ReflexSceneManager
    {
        [PublicAPI]
        public static void PreInstallScene(Scene scene, Action<ContainerBuilder> builder)
        {
            UnityInjector.ScenePreInstaller.Add(scene.path, builder);
        }
        [PublicAPI]
        public static void PreInstallScene(string scenePath, Action<ContainerBuilder> builder)
        {
            UnityInjector.ScenePreInstaller.Add(scenePath, builder);
        }
        
        [PublicAPI]
        public static void OverrideSceneParentContainer(Scene scene, Container parent)
        {
            UnityInjector.SceneContainerParentOverride.Add(scene.path, parent);
        }
        [PublicAPI]
        public static void OverrideSceneParentContainer(string scenePath, Container parent)
        {
            UnityInjector.SceneContainerParentOverride.Add(scenePath, parent);
        }
    }
}