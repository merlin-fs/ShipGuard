using System;
using System.Collections.Generic;
using System.Linq;

using Game.Core.Contexts;

using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Game.Model.Locations
{
    public class LocationScenes : ScriptableBindConfig
    {
        [SerializeField] private LocationItem[] locations;

        private Dictionary<string, LocationItem> m_Locations;

        public IEnumerable<AssetReference> Scenes => m_Locations.Values.Select(iter => iter.Scene);

        public bool TryGetSceneLocation(string name, out AssetReference sceneRef)
        {
            sceneRef = default;
            if (!m_Locations.TryGetValue(name, out var item)) return false;
            sceneRef = item.Scene;
            return true;
        }
        
        private void Awake()
        {
            m_Locations = BuildDictionary();
        }

        private void OnValidate()
        {
            m_Locations = BuildDictionary();
        }

        private Dictionary<string, LocationItem> BuildDictionary()
        {
            return locations?.ToDictionary(iter => iter.Name, iter => iter) ?? new ();
        }
        
        [Serializable]
        private struct LocationItem
        {
            public AssetReference Scene;
            public string Name;
        }
    }
}
