using System;

using Game.Core.HybridTransforms;
using Game.Views;

using Unity.Entities;

namespace Game.Core.Spawns
{
    public partial struct Spawn
    {
        [UpdateInGroup(typeof(GameEndSystemGroup), OrderLast = true)]
        public partial struct CleanupSystem : ISystem
        {
            EntityQuery m_Query;

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
                var system = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
                var ecb = system.CreateCommandBuffer(state.WorldUnmanaged);
                ecb.RemoveComponent<Spawn.PostTag>(m_Query, EntityQueryCaptureMode.AtPlayback);
            }
        }
    }
}