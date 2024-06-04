using System.Collections;
using System.Collections.Generic;

using Game.Core.Defs;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Core.Loading
{
    public class LoadPrefabRepositories : LoadRepositories<GameObject>
    {
        protected override AsyncOperationHandle<IList<GameObject>> GetAsyncOperationHandle(IEnumerable keys)
        {
            return Addressables.LoadAssetsAsync<GameObject>(keys, null);
        }

        protected override IEnumerable<IConfig> CastToConfig(IEnumerable<GameObject> result)
        {
            return null;
            /*
            return result.Select(obj =>
            {
                var environment = obj.GetComponent<Map.IPlacement>();
                var def = new Structure.StructureDef 
                {
                    Size = environment.Size,
                    Pivot = environment.Pivot,
                    Layer = environment.Layer,
                };
                var prefab = obj.GetComponent<PrefabEnvironmentAuthoring>();
                return new StructureConfig(prefab.ID, prefab, def);
            });
            */
        }
    }
}