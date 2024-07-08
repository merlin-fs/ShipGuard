using System;

using Common.Defs;

using Game.Core.Defs;
using Game.Core.Spawns;

using Unity.Entities;

namespace Game.Model.Locations
{
    public struct PlayerSpawnPoint : IDefinable<PlayerSpawnPoint.Def>, IComponentData, IDefinableCallback, IStorageData
    {
        public bool Enabled { get; set; } 
        public RefLink<Def> RefLink { get; private set; }

        public void SetDef(ref RefLink<Def> link)
        {
            RefLink = link;
        }

        public void AddComponentData(Entity entity, IDefinableContext context)
        {
            context.AddComponentData(entity, this);
            context.AddComponentData(entity, new Spawn.ViewAttachTag());
            context.AddComponentData(entity, new LocationTag());
            
            //TODO: доробити StorageData
            context.AddComponentData(entity, new TUserStorageDataTag());
        }

        [Serializable]
        public class Def : IDef
        {
            public int GetTypeIndexDefinable() => ((ComponentType)typeof(PlayerSpawnPoint)).TypeIndex;
        }
    }
}
