using System;

using Game.Core.Defs;
using Game.Core.Repositories;
using Game.Model;

using Unity.Entities;

using Reflex.Attributes;

using Unity.Transforms;

using UnityEngine;

namespace Game.Core.Spawns
{
    public partial struct Spawn
    {
        [UpdateInGroup(typeof(GameSpawnSystemGroup))]
        public partial struct System : ISystem
        {
            private static string m_PrefabType;
            
            [Inject] private static ConfigRepository m_Repository;
            [Inject] private static GameEntityRepository m_GameEntityRepository;
            
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
                    Debug.Log($"<color=green>[Spawn]</color> Condition {entity}, {gameEntity.ID} : {!spawn.ValueRO.DontCreate}");
                    if (spawn.ValueRO.DontCreate)
                    {
                        ecb.DestroyEntity(entity); 
                    }
                }

                //With Config
                foreach (var (spawn, configInfo, entity) in SystemAPI.Query<Spawn, ConfigInfo>()
                             .WithAll<WithDataTag>()
                             .WithEntityAccess())
                {
                    if (spawn.DontCreate) continue;
                        
                    Debug.Log($"<color=green>[Spawn]</color> Setup config {entity} : {configInfo.ConfigId}");
                    var config = m_Repository.FindByID(configInfo.ConfigId);
                    if (config == null) throw new ArgumentNullException($"Config {configInfo.ConfigId} not found");
                    config.Configure(entity, state.EntityManager, new CommandBufferContext(ecb));
                }

                // - Init GameEntity
                foreach (var (spawn, gameEntity, entity) in SystemAPI.Query<Spawn, RefRW<GameEntity>>()
                             .WithEntityAccess())
                {
                    if (spawn.DontCreate) continue;

                    Debug.Log($"<color=green>[Spawn]</color> Init GameEntity {entity} : {gameEntity.ValueRO.ID}");
                    gameEntity.ValueRW.Initialization(entity);
                    m_GameEntityRepository.Insert(gameEntity.ValueRO.ID, gameEntity.ValueRO);
                    postEcb.AddComponent<Spawn.PostTag>(entity);
                    ecb.RemoveComponent<Spawn>(entity);
                }
                
                foreach (var (eventDone, gameEntity, entity) in SystemAPI.Query<Event, GameEntity>().WithAll<Spawn>()
                             .WithEntityAccess())
                {
                    Debug.Log($"<color=green>[Spawn]</color> Event {entity} : {gameEntity.ID}");
                    eventDone.Callback.Invoke(gameEntity);
                    ecb.RemoveComponent<Event>(entity);
                }
            }
        }
    }
}