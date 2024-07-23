using System;
using System.Pool;

using Common.Core;

using Game.Core.Defs;
using Game.Model;
using Game.Model.Locations;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using Game.Storages;
using Game.Views;

namespace Game.Core.Spawns
{
    public class Spawner: IDisposable
    {
        private static readonly ObjectPool<Spawner> m_Pool = new(_ => new Spawner());
        
        private Entity m_Entity;
        private EntityCommandBuffer m_Ecb;
        
        private Spawner(){}

        public static Spawner Setup(EntityCommandBuffer ecb, IConfig config)
        {
            var builder = m_Pool.Get();
            builder.m_Ecb = ecb;
            builder.m_Entity = ecb.Instantiate(config.EntityPrefab);
            return builder;
        }

        public void Dispose()
        {
            m_Pool.Release(this);
        }

        public static void Destroy(EntityCommandBuffer ecb, IGameEntity gameEntity)
        {
            ecb.AddComponent<Spawn.DestroyTag>(gameEntity.Entity);
        }

        public static void RemoveWaitTag()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var ecb = entityManager.WorldUnmanaged.EntityManager.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                .CreateCommandBuffer();
            ecb.RemoveComponent<Spawn.WaitSpawnTag>(entityManager.UniversalQuery, EntityQueryCaptureMode.AtPlayback);
        }
        
        public Spawner WithNewView()
        {
            m_Ecb.AddComponent<Spawn.ViewTag>(m_Entity);
            return this;
        }

        public Spawner WithStorage<T>()
            where T : unmanaged, IStorageContainerType, IComponentData 
        {
            m_Ecb.AddComponent<T>(m_Entity);
            return this;
        }
        
        public Spawner WithId(Uuid id)
        {
            m_Ecb.AddComponent(m_Entity, new GameEntity(id, Entity.Null));
            return this;
        }

        public Spawner WhereCondition(Func<GameEntity, bool> condition)
        {
            m_Ecb.AddComponent(m_Entity, new Spawn.Condition{ Value = condition });
            return this;
        }

        public Spawner WithPosition(float3 value)
        {
            m_Ecb.AddComponent(m_Entity, new LocalTransform{ Position = value });
            return this;
        }

        public Spawner WithLocationPoint(Uuid locationPointId)
        {
            m_Ecb.AddComponent(m_Entity, new LocationLink{ LocationId = locationPointId });
            return this;
        }
        
        public Spawner WithEvent(Action<GameEntity> onDone)
        {
            m_Ecb.AddComponent<Spawn.Event>(m_Entity,
                new Spawn.Event()
                {
                    Callback = onDone,
                });
            return this;
        }
    }
}
