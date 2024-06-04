using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Common.Core;

using Game.Core.Defs;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Core.Loading
{
    public class LoadConfigRepositories : LoadRepositories<IIdentifiable<ObjectID>>
    {
        protected override AsyncOperationHandle<IList<IIdentifiable<ObjectID>>> GetAsyncOperationHandle(IEnumerable keys)
        {
            return Addressables.LoadAssetsAsync<IIdentifiable<ObjectID>>(keys, null);
        }

        protected override IEnumerable<IConfig> CastToConfig(IEnumerable<IIdentifiable<ObjectID>> result)
        {
            return result.Cast<IConfig>();
        }
    }
}