using Game.Core.Repositories;
using Game.Debugs.Gizmos;

using Reflex.Attributes;

using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

using UnityEngine;

namespace Game.Model
{
    public partial struct Target
    {
        [UpdateInGroup(typeof(GameLogicSystemGroup))]
        public partial class FinderSystem : SystemBase
        {
            private EntityQuery m_Query;
            private EntityCommandBufferSystem m_EcbSystem;
            [Inject] private static InteractionViewRepository m_InteractionViewRepository;
            
            protected override void OnCreate()
            {
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<UseFinderTag, LocalToWorld>()//Query, 
                    .Build();

                RequireForUpdate(m_Query);
                m_EcbSystem = EntityManager.World.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
            }

            protected override void OnUpdate()
            {
                const int maxHits = 10;
                
                var ecb = m_EcbSystem.CreateCommandBuffer().AsParallelWriter();
                var overlapSphereCommands = new NativeList<OverlapSphereCommand>(Allocator.TempJob);
                var entities = m_Query.ToEntityArray(Allocator.TempJob);
                var results = new NativeArray<ColliderHit>(entities.Length * maxHits, Allocator.TempJob);
                //var query = m_Query.ToComponentDataArray<Query>(Allocator.TempJob);
                var localToWorlds = m_Query.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);

                var gizmos = GetSingletonRW<GizmosSystem.Singleton>().ValueRW
                    .CreateCommandBuffer().AsParallelWriter();
                //var Gizmos = gizmos.ValueRW.CreateCommandBuffer().AsParallelWriter(),
                    
                var queryParameters = new QueryParameters 
                {
                    hitTriggers = QueryTriggerInteraction.UseGlobal,
                    layerMask = -1,
                };
                
                var handle = Job.WithCode(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                    {
                        
                        var sphereCenter = localToWorlds[i].Position;
                        var sphereRadius = 10f;//query[i].Radius;
                        gizmos.DrawSolidDisc(sphereCenter, math.up(), sphereRadius, new Color(0.1f, 0f, 0f, 0.15f));
                        
                        overlapSphereCommands.Add(new OverlapSphereCommand(sphereCenter, sphereRadius, queryParameters));
                    }
                    
                }).Schedule(Dependency);        
                handle = localToWorlds.Dispose(handle);
                handle.Complete();
                
                Dependency = OverlapSphereCommand.ScheduleBatch(overlapSphereCommands, results, 1, maxHits, Dependency);

                Dependency = Job.WithoutBurst().WithCode(() =>
                {
                    for (int i = 0; i < entities.Length; i++)
                    {
                        var buffer = ecb.AddBuffer<PossibleTargets>(0, entities[i]);
                        buffer.Clear();
                        for (int j = 0; j < maxHits; j++)
                        {
                            var instanceID = results[(i * maxHits) + j].instanceID;
                            var view = m_InteractionViewRepository.FindByID(instanceID);
                            if (view == null || view.GameEntity.Entity == entities[i]) continue;
                            buffer.Add(view.GameEntity.Entity);
                        }
                    }
                }).Schedule(Dependency);
                
                Dependency = entities.Dispose(Dependency);
                Dependency = overlapSphereCommands.Dispose(Dependency);
                Dependency = results.Dispose(Dependency);
                //m_EcbSystem.AddJobHandleForProducer(Dependency);
            }
        }
    }
}
