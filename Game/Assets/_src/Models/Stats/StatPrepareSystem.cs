using System;
using Unity.Entities;

using Game.Core.Repositories;

using Reflex.Attributes;

namespace Game.Model.Stats
{
    using Core.Defs;
    
    [UpdateInGroup(typeof(GameLogicInitSystemGroup), OrderFirst = true)]
    public partial struct StatPrepareSystem : ISystem
    {
        private EntityQuery m_Query;
        [Inject] private static ObjectRepository m_Repository;

        public void OnCreate(ref SystemState state)
        {
            
            m_Query = SystemAPI.QueryBuilder()
                .WithAll<PrepareStat>()
                .WithAllRW<Stat>()
                .Build();
            state.RequireForUpdate(m_Query);
        }

        partial struct PrepareStatsJob : IJobEntity
        {
            void Execute(in DynamicBuffer<PrepareStat> configs, ref DynamicBuffer<Stat> stats)
            {
                var repo = m_Repository;
                foreach (var iter in configs)
                {
                    var config = repo.FindByID(iter.ConfigID);
                    if (config is IConfigStats statConfig)
                    {
                        statConfig.Configure(stats);
                    }
                }
            }
        }

        public void OnUpdate(ref SystemState state)
        {
            var system = state.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>();
            var ecb = system.CreateCommandBuffer();
            state.Dependency = new PrepareStatsJob()
            {
            }.ScheduleParallel(m_Query, state.Dependency);

            ecb.RemoveComponent<PrepareStat>(m_Query, EntityQueryCaptureMode.AtPlayback);
        }
    }
}