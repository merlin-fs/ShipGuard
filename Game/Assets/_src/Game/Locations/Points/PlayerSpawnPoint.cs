using System;

using Common.Defs;

using Game.Core.Defs;

using Unity.Entities;
using Unity.Transforms;

namespace Game.Model.Locations
{
    [Serializable]
    public struct PlayerSpawnPoint : ILocationItem, IDefinable<PointConfig.Def>, IComponentData, IDefinableCallback
    {
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
            context.AddComponentData(entity, new LocalTransform());
            context.AddComponentData(entity, new LocalToWorld());
        }
    }
}
