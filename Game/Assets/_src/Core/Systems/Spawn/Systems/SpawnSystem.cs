using System;

using Game.Core.Defs;
using Game.Core.Repositories;
using Game.Model;

using Unity.Entities;

using Reflex.Attributes;

namespace Game.Core.Spawns
{
    public partial struct Spawn
    {
        [UpdateInGroup(typeof(GameSpawnSystemGroup))]
        public partial struct System : ISystem
        {
            private static string m_PrefabType;
            
            [Inject] private static ConfigRepository m_Repository;
            
            private EntityQuery m_Query; 
            
            public void OnCreate(ref SystemState state)
            {
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<Spawn>()
                    .Build();
            }

            public void OnUpdate(ref SystemState state)
            {
                if (m_Query.IsEmpty) return;
                
                var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);
                var postEcb = SystemAPI.GetSingleton<GameSpawnSystemCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);
                    
                    

                foreach (var (spawn, gameEntity, condition, entity) in SystemAPI.Query<RefRW<Spawn>, GameEntity, Condition>()
                             .WithEntityAccess())
                {
                    spawn.ValueRW.DontCreate = !condition.Value.Invoke(gameEntity);
                    if (spawn.ValueRO.DontCreate)
                    {
                        ecb.DestroyEntity(entity); 
                    }
                }

                //With Config
                foreach (var (spawn, configInfo, entity) in SystemAPI.Query<Spawn, ConfigInfo>()
                             .WithEntityAccess())
                {
                    if (spawn.DontCreate) continue;
                        
                    var config = m_Repository.FindByID(configInfo.ConfigId);
                    if (config == null) throw new ArgumentNullException($"Prefab {configInfo.ConfigId} not found");
                    
                    config.Configure(entity, state.EntityManager, new CommandBufferContext(ecb));
                }

                // - Init GameEntity
                foreach (var (spawn, gameEntity, entity) in SystemAPI.Query<Spawn, GameEntity>()
                             .WithEntityAccess())
                {
                    if (spawn.DontCreate) continue;
                    gameEntity.Initialization(entity);
                    postEcb.AddComponent<Spawn.PostTag>(entity);
                }
                
                foreach (var (eventDone, gameEntity, entity) in SystemAPI.Query<Event, GameEntity>().WithAll<Spawn>()
                             .WithEntityAccess())
                {
                    eventDone.Callback.Invoke(gameEntity);
                    ecb.RemoveComponent<Event>(entity);
                }
                ecb.RemoveComponent<Spawn>(m_Query, EntityQueryCaptureMode.AtPlayback);
            }
        }
    }
}