using System;

using Common.Defs;

using Game.Core.Defs;

using Unity.Entities;

namespace Game.Model.Locations
{
    public struct PlayerSpawnPoint : IDefinable, IComponentData, IDefinableCallback
    {
        public RefLink<PlayerSpawnPointDef> RefLink { get; }

        public PlayerSpawnPoint(RefLink<PlayerSpawnPointDef> config)
        {
            RefLink = config;
        }
        
        public void AddComponentData(Entity entity, IDefinableContext context)
        {
            
        }

        public void RemoveComponentData(Entity entity, IDefinableContext context)
        {
            
        }
        
        [Serializable]
        public class PlayerSpawnPointDef : IDef<PlayerSpawnPoint>
        {
            
        }
    }
}
