using System;

using Game.Core.Defs;

using Unity.Entities;

namespace Game.Model.Locations
{
    public class PlayerSpawnPointConfig: GameObjectConfig
    {
        public PlayerSpawnPoint.Def Value = new ();
        protected override void Configure(Entity prefab, EntityManager manager, IDefinableContext context)
        {
            base.Configure(prefab, manager, context);
            Value.AddComponentData(prefab, manager, context);
        }
        public override ComponentTypeSet GetComponentTypeSet() => new ComponentTypeSet(ComponentType.FromTypeIndex(Value.GetTypeIndexDefinable()));
    }
}
