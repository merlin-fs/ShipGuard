using Game.AI.GOAP;
using Game.Model.Units;

using Unity.Entities;

namespace Game.Model.AI
{
    public struct UnitFindEnemyQuery : Logic.IAction
    {
        public void Execute(Logic.ExecuteContext context, EntityCommandBuffer.ParallelWriter ecb, int sortKey, Entity entity)
        {
            //if (!context.EntityManager.HasComponent<Unit>(entity)) return;
            if (!context.EntityManager.HasBuffer<Stats.Stat>(entity)) return;
            //var unit = context.EntityManager.GetComponentData<Unit>(entity);
            var stats = context.EntityManager.GetBuffer<Stats.Stat>(entity, true);
            if (!Stats.Stat.TryGetStat<Unit.AggroRange>(stats, out var aggroRange)) return;
            
            var query = new Target.Query
            {
                Radius = aggroRange.Value,
            };
            ecb.SetComponent(sortKey, entity, query);
        }
    }
}
