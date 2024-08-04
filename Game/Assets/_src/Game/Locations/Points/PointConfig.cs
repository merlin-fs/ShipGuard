using System;

using Common.Defs;

using Game.Core;
using Game.Core.Defs;

using Unity.Entities;

using UnityEngine;

namespace Game.Model.Locations
{
    public class PointConfig: GameObjectConfigWithDef<PointConfig.Def>
    {
        protected override void Configure(Entity prefab, EntityManager manager, IDefinableContext context)
        {
            base.Configure(prefab, manager, context);
            Value.AddComponentData(prefab, manager, context);
        }

        [Serializable]
        public class Def : IDef
        {
            [SerializeField, SelectType(typeof(ILocationItem))]
            private string storeType;

            public int GetTypeIndexDefinable() => storeType.GetTypeIndex();
        }
    }
}
