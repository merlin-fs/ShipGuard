using Common.Core;
using Common.Defs;

using Game.Core.Defs;
using Game.Core.Spawns;
using Game.Model.Units;
using Game.Storages;

using Unity.Entities;
using Unity.Transforms;

using UnityEngine;

namespace Game.Model.Locations
{
    public struct MobSpawnPoint : IDefinable<PointConfig.Def>, ILocationItem, IComponentData,  
        IInitializationComponentData, IDefinableCallback
    {
        [SerializeField, SelectObjectIDAttribute(typeof(UnitConfig))]
        private ObjectID m_MobConfigId;

        public int Count;
        
        public RefLink<PointConfig.Def> RefLink { get; private set; }

        public void SetDef(ref RefLink<PointConfig.Def> link)
        {
            RefLink = link;
        }

        public void Initialization(IContainer container, EntityCommandBuffer ecb, Entity entity)
        {
            ecb.SetComponent(entity, this);
            var unitManager = container.Resolve<UnitManager>(); 
            SpawnMobs(unitManager, ecb, entity);
        }

        public void AddComponentData(Entity entity, IDefinableContext context)
        {
            context.AddComponentData(entity, this);
            context.AddComponentData(entity, new LocationTag());
            context.AddComponentData(entity, new LocalTransform());
            context.AddComponentData(entity, new LocalToWorld());
        }

        private void SpawnMobs(UnitManager unitManager, EntityCommandBuffer ecb, Entity spawnPointEntity)
        {
            unitManager.SpawnMobs(ecb, spawnPointEntity, m_MobConfigId, Count);
        }
    }
}
