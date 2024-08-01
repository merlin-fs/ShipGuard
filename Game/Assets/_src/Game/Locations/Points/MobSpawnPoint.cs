using System;

using Common.Defs;

using Game.Core.Defs;

using Unity.Entities;

using UnityEngine;

namespace Game.Model.Locations
{
    //[Serializable]
    public struct MobSpawnPoint : ILocationItem, IDefinable<PointConfig.Def>, IComponentData, IDefinableCallback
    {
        [field: SerializeField]
        public bool Enabled { get; set; } 
        
        public RefLink<PointConfig.Def> RefLink { get; private set; }

        public void SetDef(ref RefLink<PointConfig.Def> link)
        {
            RefLink = link;
        }

        public void AddComponentData(Entity entity, IDefinableContext context)
        {
            context.AddComponentData(entity, this);
            context.AddComponentData(entity, new LocationTag());
        }
    }
}
