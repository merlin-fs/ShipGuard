using System.Collections.Concurrent;

using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

namespace Game.AI.GOAP
{
    public partial struct Logic
    {
        [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
        [UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
        public partial class WorldStateSystem : SystemBase
        {
            private static ConcurrentQueue<QueueItem> m_Queue;
            private BufferLookup<WorldStateComponent> m_WorldStates;
            private ComponentLookup<Logic> m_Logics;

            public void ChangeWorldState(Entity entity, WorldStateHandle value)
            {
                m_Queue.Enqueue(new QueueItem{ Entity = entity, Value = value });
            }

            protected override void OnCreate()
            {
                m_WorldStates = GetBufferLookup<WorldStateComponent>(false);
                m_Logics = GetComponentLookup<Logic>(true);
                m_Queue = new ConcurrentQueue<QueueItem>();
            }

            protected override void OnUpdate()
            {
                if (m_Queue.Count == 0)
                    return;
                
                m_WorldStates.Update(this);
                m_Logics.Update(this);

                var system = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                var ecb = system.CreateCommandBuffer(this.World.Unmanaged);
                
                //TODO: нужно добавить блокироваку.
                var items = new NativeArray<QueueItem>(m_Queue.ToArray(), Allocator.TempJob);
                m_Queue.Clear();
                
                Dependency = new SystemJob {
                    WorldStates = m_WorldStates,
                    Logics = m_Logics,
                    Items = items,
                    Writer = ecb.AsParallelWriter(),
                }.Schedule(items.Length, 5, Dependency);
                items.Dispose(Dependency);
            }

            private struct QueueItem
            {
                public Entity Entity;
                public WorldStateHandle Value;
            }

            private unsafe struct SystemJob : IJobParallelFor
            {
                [ReadOnly] public BufferLookup<WorldStateComponent> WorldStates;
                [ReadOnly] public ComponentLookup<Logic> Logics;
                public NativeArray<QueueItem> Items;
                public EntityCommandBuffer.ParallelWriter Writer;
                
                public void Execute(int index)
                {
                    var item = Items[index];
                    var logic = Logics[item.Entity];
                    var states = WorldStates[item.Entity];
                    var stateIndex = logic.Def.WorldStateMapping(item.Value.Enum);
                    
                    ref var element = ref UnsafeUtility.ArrayElementAsRef<WorldStateComponent>(states.GetUnsafeReadOnlyPtr(), stateIndex);
                    element.Value = item.Value;
                    
                    Writer.SetComponent(index, item.Entity, new ChangeTag());
                }
            }
        }
    }
}
