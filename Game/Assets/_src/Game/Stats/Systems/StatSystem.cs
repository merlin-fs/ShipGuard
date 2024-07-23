using System;
using Unity.Burst;
using Unity.Entities;

namespace Game.Model.Stats
{
    //[UpdateInGroup(typeof(GameLogicSystemGroup), OrderFirst = true)]
    public partial struct StatSystem : ISystem
    {
        private EntityQuery m_Query;

        public void OnCreate(ref SystemState state)
        {
            m_Query = SystemAPI.QueryBuilder()
                .WithAspect<StatAspect>()
                .Build();

            m_Query.AddChangedVersionFilter(ComponentType.ReadOnly<Modifier>());
            state.RequireForUpdate(m_Query);
        }

        partial struct SystemJob : IJobEntity
        {
            public float Delta;

            private void Execute(StatAspect stats)
            {
                stats.Estimation(Delta);
            }
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new SystemJob()
            {
                Delta = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel(m_Query, state.Dependency);
        }
    }
}