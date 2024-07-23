using Common.Defs;

using Game.Core.Defs;

using Unity.Entities;

namespace Game.Model.Locations
{
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
        }
    }
}
