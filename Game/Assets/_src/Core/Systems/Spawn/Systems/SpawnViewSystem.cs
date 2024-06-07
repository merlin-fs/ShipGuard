using System;
using System.Collections.Generic;

using Game.Core.Defs;
using Game.Core.HybridTransforms;
using Game.Core.Repositories;
using Game.Views;

using Reflex.Core;
using Reflex.Attributes;

using Unity.Entities;

namespace Game.Core.Spawns
{
    public partial class Spawn
    {
        [UpdateInGroup(typeof(GameSpawnSystemGroup))]
        [UpdateBefore(typeof(CleanupSystem))]
        public partial struct SpawnViewSystem : ISystem
        {
            [Inject] private static ObjectRepository m_Repository;
            [Inject] private static Container m_Container;
            [Inject] private static Spawner m_Spawner;
            
            private EntityQuery m_Query;

            public void OnCreate(ref SystemState state)
            {
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<ViewTag, Tag, PrefabInfo.BakedInnerPathPrefab>()
                    .Build();
                state.RequireForUpdate(m_Query);
            }

            public void OnUpdate(ref SystemState state)
            {
                var system = SystemAPI.GetSingleton<GameSpawnSystemCommandBufferSystem.Singleton>();
                var ecb = system.CreateCommandBuffer(state.WorldUnmanaged);

                var dictionary = new Dictionary<Entity, Container>();
                
                foreach (var (info, children, entity) in SystemAPI
                             .Query<PrefabInfo, DynamicBuffer<PrefabInfo.BakedInnerPathPrefab>>()
                             .WithAll<Tag, ViewTag>().WithEntityAccess()) 
                {
                    var config = m_Repository.FindByID(info.ConfigID);
                    if (config == null) throw new ArgumentNullException($"Prefab {info.ConfigID} not found");

                    
                    var ctx = m_Spawner.CreateContext(entity, config, ecb, children);
                    dictionary.Add(entity, ctx);
                }
                
                foreach (var (eventDone, entity) in SystemAPI
                             .Query<Event>()
                             .WithAll<Tag>().WithEntityAccess())
                {
                    if (!dictionary.TryGetValue(entity, out var ctx)) continue;
                    var view = ctx.Resolve<IView>();
                    eventDone.Callback.Invoke(view);
                }
                
                ecb.RemoveComponent<ViewTag>(m_Query, EntityQueryCaptureMode.AtPlayback);
            }
        }
    }
}