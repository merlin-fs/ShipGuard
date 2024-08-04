using System;

using Game.Core.Defs;
using Game.Core.Repositories;
using Game.Model;

using Unity.Entities;

using Reflex.Attributes;

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
            [Inject] private static GameUniqueEntityRepository m_GameUniqueEntityRepository;
            [Inject] private static ConfigRepository m_ConfigRepository;
            
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

                /*
                foreach (var (configInfo, @async, entity) in SystemAPI.Query<ConfigInfo, Async>()
                             .WithAll<Spawn>()
                             .WithEntityAccess())
                {
                    Debug.Log($"<color=green>[Spawn]</color> Async setup config {entity} : {configInfo.ConfigId}");
                    var config = m_Repository.FindByID(configInfo.ConfigId);
                    if (config == null) throw new ArgumentNullException($"Config {configInfo.ConfigId} not found");
                    using (var spawner = Spawner.Setup(ecb, config))
                    {
                        @async.Callback.Invoke(spawner);
                    }
                    ecb.DestroyEntity(entity);
                }
                */

                //With Config
                foreach (var (configInfo, entity) in SystemAPI.Query<ConfigInfo>()
                             .WithAll<WithDataTag, Spawn>()
                             .WithEntityAccess())
                {
                    Debug.Log($"<color=green>[Spawn]</color> Setup config {entity} : {configInfo.ConfigId}");
                    var config = m_Repository.FindByID(configInfo.ConfigId);
                    if (config == null) throw new ArgumentNullException($"Config {configInfo.ConfigId} not found");
                    config.Configure(entity, state.EntityManager, new CommandBufferContext(ecb));
                }

                // - Init GameEntity
                foreach (var (gameEntity, entity) in SystemAPI.Query<RefRW<GameEntity>>()
                             .WithAll<Spawn>()
                             .WithEntityAccess())
                {
                    Debug.Log($"<color=green>[Spawn]</color> Init GameEntity {entity}");
                    gameEntity.ValueRW.Initialization(entity);
                    postEcb.AddComponent<Spawn.PostTag>(entity);
                    ecb.RemoveComponent<Spawn>(entity);
                }

                foreach (var (gameEntity, uniqueEntity, entity) in SystemAPI.Query<GameEntity, UniqueEntity>()
                             .WithAll<Spawn>()
                             .WithEntityAccess())
                {
                    Debug.Log($"<color=green>[Spawn]</color> Init UniqueEntity {entity} : {uniqueEntity.DebugId}");
                    m_GameUniqueEntityRepository.Insert(uniqueEntity.ID, gameEntity);
                }
                
                foreach (var (eventDone, gameEntity, entity) in SystemAPI.Query<Event, GameEntity>()
                             .WithAll<Spawn>()
                             .WithEntityAccess())
                {
                    Debug.Log($"<color=green>[Spawn]</color> Event {entity}");
                    eventDone.Callback.Invoke(gameEntity);
                    ecb.RemoveComponent<Event>(entity);
                }
            }
        }
    }
}