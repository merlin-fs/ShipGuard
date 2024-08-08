using Game.AI.GOAP;

using Unity.Entities;
using Unity.Transforms;

namespace Game.Model.AI
{
    public struct WeaponsActivate : Logic.IAction
    {
        public void Execute(Logic.ExecuteContext context, EntityCommandBuffer.ParallelWriter ecb, Entity entity)
        {
            if (!context.EntityManager.HasBuffer<Child>(entity)) return;

            var children = context.EntityManager.GetBuffer<Child>(entity, true);
            foreach (var iter in children)
            {
                //context.SetWorldState(iter.Value, Weapon.State.Active, true);
            }
        }
    }
}
