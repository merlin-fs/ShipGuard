using System.Collections.Generic;

using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Game.Model.Locations
{
    public class LocationScenes : ScriptableObject
    {
        [SerializeField] private AssetReference[] locationScenes;

        public IEnumerable<AssetReference> Scenes => locationScenes;
    }
}
