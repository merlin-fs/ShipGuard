using System;
using System.Collections.Generic;
using System.Linq;

using Game.Core.Contexts;

using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Model.Locations
{
    [Serializable]
    public class LocationSceneItem
    {
        [field:SerializeField] public string Name { get; private set; }
        [field:SerializeField] public AssetReference SceneReference { get; private set; }
        [field:SerializeField] public LoadSceneMode LoadSceneMode { get; private set; }
    }
    
    public class LocationScenes : ScriptableBindConfig
    {
        [SerializeField] private LocationSceneItem[] locations;

        private Dictionary<string, LocationSceneItem> m_Locations;
        
        public IEnumerable<AssetReference> Scenes => m_Locations.Values.Select(iter => iter.SceneReference);

        public bool TryGetSceneLocation(string locationName, out LocationSceneItem sceneItem)
        {
            return m_Locations.TryGetValue(locationName, out sceneItem);
        }

        private void Awake()
        {
            m_Locations = BuildDictionary();
        }

        private void OnValidate()
        {
            m_Locations = BuildDictionary();
        }

        private Dictionary<string, LocationSceneItem> BuildDictionary()
        {
            return locations?.ToDictionary(iter => iter.Name, iter => iter) ?? new();
        }
    }
}
