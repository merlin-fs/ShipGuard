using System;

using Game.Core.Repositories;

using Unity.Entities;

using Reflex.Attributes;
using Reflex.Core;

namespace Game.Core.Spawns
{
    public partial class Spawn
    {
        [UpdateInGroup(typeof(GameSpawnSystemGroup))]
        partial struct System : ISystem
        {
            private EntityQuery m_Query;
            private static string m_PrefabType;
            
            [Inject] private static ObjectRepository m_Repository;
            [Inject] private static Container m_Container;
            [Inject] private static Spawner m_Spawner;
            
            public void OnCreate(ref SystemState state)
            {
                m_Query = SystemAPI.QueryBuilder()
                    .WithAll<Spawn, Component>()
                    .Build();
                state.RequireForUpdate(m_Query);
            }

            public void OnUpdate(ref SystemState state)
            {
                var system = SystemAPI.GetSingleton<GameSpawnSystemCommandBufferSystem.Singleton>();
                var ecb = system.CreateCommandBuffer(state.WorldUnmanaged);

                foreach (var (spawn, entity) in SystemAPI.Query<Spawn>().WithEntityAccess())
                {
                    var components = SystemAPI.GetBuffer<Component>(entity);
                    var config = m_Repository.FindByID(spawn.PrefabID);
                    if (config == null) throw new ArgumentNullException($"Prefab {spawn.PrefabID} not found");

                    var builder = m_Spawner.Spawn(config, ecb)
                        .WithView()
                        .WithLogicEnabled(false)
                        .WithComponents(components);
                        
                    if (spawn.Data != null)
                    {
                        builder.WithData(spawn.Data);
                    }
                    ecb.DestroyEntity(entity);
                }
                
                /*
                m_LookupTransform.Update(ref state);
                var map = SystemAPI.GetAspect<Map.Aspect>(m_QueryMap.GetSingletonEntity());
                var system = state.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>();
                
                state.Dependency = new SystemJob
                {
                    Map = map, 
                    LookupTransform = m_LookupTransform, 
                    Writer = system.CreateCommandBuffer().AsParallelWriter(),
                }.ScheduleParallel(m_Query, state.Dependency);
                //state.Dependency.Complete();
                var ecb = system.CreateCommandBuffer();
                ecb.DestroyEntity(m_Query, EntityQueryCaptureMode.AtPlayback);
                */
            }

            /*
            partial struct SystemJob : IJobEntity
            {
                [NativeDisableParallelForRestriction, NativeDisableUnsafePtrRestriction]
                public Map.Aspect Map;

                [ReadOnly] public ComponentLookup<LocalTransform> LookupTransform;
                public EntityCommandBuffer.ParallelWriter Writer;

                void Execute([EntityIndexInQuery] int idx, in Spawn spawn, in DynamicBuffer<Component> components)
                {
                    var inst = Writer.Instantiate(idx, spawn.Prefab);

                    var transform = LookupTransform[spawn.Prefab];
                    transform.Position = Map.Value.MapToWord(spawn.Position);

                    Writer.AddComponent<Tag>(idx, inst);
                    Writer.AddComponent(idx, inst, transform);
                    foreach (var iter in components)
                        Writer.AddComponent(idx, inst, iter.ComponentType);
                }
            }
            */
        }
    }
}