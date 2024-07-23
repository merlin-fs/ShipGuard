using System;
using System.Collections.Generic;

using Common.Core;

using Game.Core.HybridTransforms;
using Game.Core.Repositories;
using Game.Model;

using Reflex.Core;

using Game.Views;

using Reflex.Attributes;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

namespace Game.Core.Spawns
{
    public partial struct Spawn
    {
        [UpdateInGroup(typeof(GameSpawnSystemGroup))]
        [UpdateAfter(typeof(Spawn.System))]
        public partial struct SpawnViewSystem : ISystem
        {
            [Inject] private static ConfigRepository m_Repository;
            [Inject] private static Container m_Container;
            [Inject] private static IViewFactory m_ViewFactory;
            
            private EntityQuery m_Query;

            public void OnCreate(ref SystemState state)
            {
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<Spawn.PostTag>()
                    .WithNone<WaitSpawnTag>()
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
                             .WithAll<ViewTag, Spawn.PostTag>()
                             .WithNone<WaitSpawnTag>()
                             .WithEntityAccess())
                {
                    Debug.Log($"[Spawn] New view {entity} {configInfo.ConfigId} : {gameEntity.ID}");
                    var config = m_Repository.FindByID(configInfo.ConfigId);
                    if (config == null) throw new ArgumentNullException($"Prefab {configInfo.ConfigId} not found");

                    var view = m_ViewFactory.Instantiate(config, entity, state.EntityManager, m_Container);
                    ecb.AddComponent(entity, new HybridTransform.ViewReference{ Value = view });
                    ecb.AddComponent<LocalTransform>(entity);
                    ecb.AddComponent<LocalToWorld>(entity);

                    view.Initialization(gameEntity);
                    ecb.RemoveComponent<ViewTag>(entity);
                }
            }
        }
    }
}