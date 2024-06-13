using System.Collections.Generic;

using Common.Core.Loading;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Core.Loading
{
    public class LoadingBindConfig : ScriptableObject
    {
        [SerializeField] private AssetReference mainSceneRef;
        
        public IEnumerable<LoadingManager.CommandItem> GetCommands()
        {
            return new LoadingManager.CommandItem[] {
                //0
                new(new LoadConfigRepositories("defs")),
                //1
                new(new LoadPrefabRepositories("defs")),
                //2
                new(new LoadScene(mainSceneRef)),
                //3
                new(new LoadEntitiesWorld(), 0, 1, 2),
                //4
                new(new LoadEntitiesPrefab(), 3),
                //5
                new(new LoadGameInitialization(), 4),
                //6
                new(new LoadUI(), 5),
            };
        }
    }
}
