using System;
using System.Collections.Generic;

using Common.Core;

using Game.Core.Repositories;
using Game.Model;

using Reflex.Core;

using Game.Views;

using Reflex.Attributes;

using Unity.Entities;

namespace Game.Core.Spawns
{
    public partial struct Spawn
    {
        [UpdateInGroup(typeof(GameSpawnSystemGroup))]
        [UpdateBefore(typeof(CleanupSystem))]
        public partial struct SpawnViewSystem : ISystem
        {
            [Inject] private static ConfigRepository m_Repository;
            [Inject] private static Spawner m_Spawner;
            [Inject] private static IContainer m_Container;
            
            private EntityQuery m_Query;

            public void OnCreate(ref SystemState state)
            {
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<Spawn>()
                    .Build();
                state.RequireForUpdate(m_Query);
            }

            public void OnUpdate(ref SystemState state)
            {
                if (m_Query.IsEmpty) return;
                
                var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);
                
                //New view
                foreach (var (configInfo, gameEntity, entity) in SystemAPI.Query<ConfigInfo, GameEntity>()//, children, DynamicBuffer<PrefabInfo.BakedInnerPathPrefab>
                             .WithAll<ViewTag, Spawn>()
                             .WithEntityAccess())
                {
                    var config = m_Repository.FindByID(configInfo.ConfigId);
                    if (config == null) throw new ArgumentNullException($"Prefab {configInfo.ConfigId} not found");

                    var view = m_Spawner.CreateView(entity, config, state.EntityManager, ecb);

                    view.Initialization(gameEntity);
                    ecb.RemoveComponent<ViewTag>(entity);
                }
            }
        }
    }
}