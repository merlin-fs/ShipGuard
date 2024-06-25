using System;
using System.Collections.Generic;

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
            
            private EntityQuery m_Query;
            private GameSpawnSystemCommandBufferSystem.Singleton m_SpawnSystem;

            public void OnCreate(ref SystemState state)
            {
                m_SpawnSystem = SystemAPI.GetSingleton<GameSpawnSystemCommandBufferSystem.Singleton>();
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<PostSpawnTag>()
                    .Build();
                state.RequireForUpdate(m_Query);
            }

            public void OnUpdate(ref SystemState state)
            {
                if (m_Query.IsEmpty) return;
                
                var ecb = m_SpawnSystem.CreateCommandBuffer(state.WorldUnmanaged);
                //New view
                var dictionary = new Dictionary<Entity, Container>();
                foreach (var (configInfo, entity) in SystemAPI.Query<ConfigInfo>()//, children, DynamicBuffer<PrefabInfo.BakedInnerPathPrefab>
                             .WithAll<ViewTag, PostSpawnTag>()
                             .WithEntityAccess())
                {
                    var config = m_Repository.FindByID(configInfo.ConfigId);
                    if (config == null) throw new ArgumentNullException($"Prefab {configInfo.ConfigId} not found");

                    var ctx = m_Spawner.CreateContext(entity, config, ecb);//, children
                    dictionary.Add(entity, ctx);
                }

                foreach (var (eventDone, entity) in SystemAPI.Query<Event>().WithAll<PostSpawnTag>()
                             .WithEntityAccess())
                {
                    if (!dictionary.TryGetValue(entity, out var ctx)) continue;
                    var view = ctx.Resolve<IView>();
                    eventDone.Callback.Invoke(view);
                }
                ecb.RemoveComponent<PostSpawnTag>(m_Query, EntityQueryCaptureMode.AtPlayback);
            }
        }
    }
}