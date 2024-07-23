using System;

using Common.Defs;

using Game.Core.Defs;

using Unity.Entities;

using UnityEngine;

namespace Game.Model.Locations
{
    public class PointConfig: GameObjectConfig
    {
        public Def Value = new ();
        protected override void Configure(Entity prefab, EntityManager manager, IDefinableContext context)
        {
            base.Configure(prefab, manager, context);
            Value.AddComponentData(prefab, manager, context);
        }
        public override ComponentTypeSet GetComponentTypeSet() => new ComponentTypeSet(ComponentType.FromTypeIndex(Value.GetTypeIndexDefinable()));

        [Serializable]
        public class Def : IDef
        {
            [SerializeField, SelectType(typeof(ILocationItem))]
            private string storeType;
            public int GetTypeIndexDefinable() => ((ComponentType)Type.GetType(storeType)).TypeIndex;
        }
    }
}
