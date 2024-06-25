using System;

using Game.Core.Defs;

using Unity.Entities;

namespace Game.Model.Locations
{
    public class PlayerSpawnPointConfig: GameObjectConfig
    {
        public PlayerSpawnPoint.Def Value = new ();
        protected override void Configure(Entity prefab, IDefinableContext context)
        {
            base.Configure(prefab, context);
            Value.AddComponentData(prefab, context);
        }
    }
}
