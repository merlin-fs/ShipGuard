using System;
using System.Pool;

using Common.Core;

using Game.Core.Defs;
using Game.Core.HybridTransforms;
using Game.Model;
using Game.Model.Locations;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using Game.Storages;

using Unity.Collections;

namespace Game.Core.Spawns
{
    public class Spawner: IDisposable
    {
        private static readonly ObjectPool<Spawner> m_Pool = new(_ => new Spawner());
        
        private Entity m_Entity;
        private EntityCommandBuffer m_Ecb;
        private IConfig m_Config;
        
        private Spawner(){}

        public static Spawner Setup(EntityCommandBuffer ecb, IConfig config)
        {
            return Setup(ecb, config, ecb.Instantiate(config.EntityPrefab));;
        }

        public static Spawner SetupUnique(EntityCommandBuffer ecb, IConfig config, Uuid id)
        {
            var builder = Setup(ecb, config);
            ecb.AddComponent(builder.m_Entity, new UniqueEntity(id));
            return builder;
        }

        private static Spawner Setup(EntityCommandBuffer ecb, IConfig config, Entity entity)
        {
            var builder = m_Pool.Get();
            builder.m_Ecb = ecb;
            builder.m_Entity = entity;
            builder.m_Config = config;
            ecb.AddComponent<GameEntity>(entity);
            return builder;
        }
        
        public static void Setup(EntityCommandBuffer ecb, IConfig config, int count, Action<Spawner> forEach)
        {
            var entities = CollectionHelper.CreateNativeArray<Entity>(count, Allocator.Temp);
            ecb.Instantiate(config.EntityPrefab, entities);
            for (int i = 0; i < count; i++)
            {
                using var builder = Setup(ecb, config, entities[i]);
                forEach.Invoke(builder);
            }
            entities.Dispose();
        }

        public void Dispose()
        {
            m_Pool.Release(this);
        }

        public static void Destroy(EntityCommandBuffer ecb, IGameEntity gameEntity)
        {
            ecb.AddComponent<Spawn.DestroyTag>(gameEntity.Entity);
        }

        public static void DestroyEntityOnly(EntityCommandBuffer ecb, IGameEntity gameEntity)
        {
            ecb.RemoveComponent<HybridTransform.ReferenceView>(gameEntity.Entity);
            ecb.AddComponent<Spawn.DestroyTag>(gameEntity.Entity);
        }

        public Spawner WithCallback(Action<Entity> callback)
        {
            callback?.Invoke(m_Entity);
            return this;
        }

        public static void RemoveWaitTag()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var ecb = entityManager.WorldUnmanaged.EntityManager.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                .CreateCommandBuffer();
            ecb.RemoveComponent<Spawn.WaitSpawnTag>(entityManager.UniversalQuery, EntityQueryCaptureMode.AtPlayback);
        }
        
        public Spawner WithStorage<T>()
            where T : unmanaged, IStorageContainerType, IComponentData 
        {
            m_Ecb.AddComponent<T>(m_Entity);
            return this;
        }
        
        public Spawner WithPosition(float3 value)
        {
            m_Ecb.AddComponent(m_Entity, new LocalTransform{ Position = value });
            return this;
        }

        public Spawner WithLocationPoint(Entity entity)
        {
            m_Ecb.AddComponent(m_Entity, new ReferenceLocation{ Entity = entity });
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
