using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Game.Core.Contexts;
using Game.Model.Locations;

using JetBrains.Annotations;

using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game
{
    public class LocationManager
    {
        [Inject] private Container m_Container;
        private readonly LocationScenes m_LocationScenes;

        private LocationInfo m_CurrentLocation;

        public LocationManager(LocationScenes locationScenes)
        {
            m_LocationScenes = locationScenes;
            InitLocationScenes();
        }

        public Task LoadLocation(string locationName, Action<Container> onCompleted)
        {
            if (!m_LocationScenes.TryGetSceneLocation(locationName, out var locationItem))
            {
                return Task.FromException(new ArgumentException($"Location {locationName} not found!"));
            }
            
            var task = Addressables.LoadSceneAsync(locationItem.SceneReference, locationItem.LoadSceneMode);
            task.Completed += handle =>
            {
                using var pooledObject1 = ListPool<GameObject>.Get(out var rootGameObjects);
                handle.Result.Scene.GetRootGameObjects(rootGameObjects);
                m_CurrentLocation = new LocationInfo 
                {
                    SceneInstance = handle.Result,
                    Item = locationItem,
                };

                foreach (var root in rootGameObjects)
                {
                    m_CurrentLocation.LocationRoots.AddRange(root.GetComponentsInChildren<LocationRoot>());
                }

                var container = handle.Result.Scene.GetSceneContainer();

                foreach (var root in rootGameObjects)
                {
                    root.GetComponent<LocationSceneContext>()?.Initialization(container);
                }
                ReflexSceneManager.OverrideSceneParentContainer(scene: handle.Result.Scene, parent: m_Container);
                onCompleted?.Invoke(container);
            };
            return task.Task;
        }

        private async void InitLocationScenes()
        {
            foreach (var sceneRef in m_LocationScenes.Scenes)
            {
                var loc = await Addressables.LoadResourceLocationsAsync(sceneRef).Task;
                var id = Addressables.ResourceManager.TransformInternalId(loc[0]);
                ReflexSceneManager.OverrideSceneParentContainer(scenePath: id, parent: m_Container);
            }
        }


        public Task CloseCurrentLocation(Action onCompleted)
        {
            if (m_CurrentLocation == null) 
            {
                onCompleted?.Invoke();
                return Task.CompletedTask;
            }
            var task = Addressables.UnloadSceneAsync(m_CurrentLocation.SceneInstance, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            task.Completed += handle =>
            {
                m_CurrentLocation = null;
                onCompleted?.Invoke();
            };
            return task.Task;
        }
        
        [CanBeNull]
        public LocationSceneItem CurrentLocation => m_CurrentLocation?.Item;

        [CanBeNull]
        public LocationSceneItem GetLocationItem(string locationName)
        {
            return !m_LocationScenes.TryGetSceneLocation(locationName, out var locationItem)
                ? null
                : locationItem;
        }
        
        [CanBeNull]
        public IEnumerable<LocationRoot> CurrentLocationRoots => m_CurrentLocation?.LocationRoots;

        private class LocationInfo
        {
            public LocationSceneItem Item;
            public SceneInstance SceneInstance;
            public List<LocationRoot> LocationRoots = new();
        }
    }
}
