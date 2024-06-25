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
        partial struct System : ISystem
        {
            private static string m_PrefabType;
            
            [Inject] private static ConfigRepository m_Repository;
            
            private GameSpawnSystemCommandBufferSystem.Singleton m_SpawnSystem;
            private EntityQuery m_Query; 
            
            public void OnCreate(ref SystemState state)
            {
                m_SpawnSystem = SystemAPI.GetSingleton<GameSpawnSystemCommandBufferSystem.Singleton>();
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<Spawn>()
                    .Build();
            }

            public void OnUpdate(ref SystemState state)
            {
                if (m_Query.IsEmpty) return;
                
                var ecb = m_SpawnSystem.CreateCommandBuffer(state.WorldUnmanaged);

                //With Config
                foreach (var (configInfo, entity) in SystemAPI.Query<ConfigInfo>().WithAny<Spawn>()
                             .WithEntityAccess())
                {
                    var config = m_Repository.FindByID(configInfo.ConfigId);
                    if (config == null) throw new ArgumentNullException($"Prefab {configInfo.ConfigId} not found");

                    config.Configure(entity, new CommandBufferContext(ecb));
                    
                    ecb.AddComponent<PostSpawnTag>(entity);
                    ecb.RemoveComponent<Spawn>(entity);
                }

                // - Init GameEntity
                foreach (var (gameEntity, entity) in SystemAPI.Query<GameEntity>()
                             .WithAll<Spawn, WithDataTag>().WithEntityAccess())
                {
                    gameEntity.Initialization(entity);
                }
            }
        }
    }
}