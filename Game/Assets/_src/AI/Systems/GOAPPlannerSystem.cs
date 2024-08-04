using System.Collections.Generic;

using Unity.Entities;

namespace Game.AI.GOAP
{
    public partial class GOAPPlannerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            
            foreach (var (actionQueue, goal) in SystemAPI.Query<DynamicBuffer<GOAPActionBufferElement>, GoalComponent>())
            {
                // Очистить текущую очередь действий
                actionQueue.Clear();

                // Построить план действий
                Queue<GOAPAction> plan = BuildPlan(goal);

                // Заполнить очередь действий
                while (plan.Count > 0)
                {
                    actionQueue.Add(new GOAPActionBufferElement {action = plan.Dequeue()});
                }
            }
        }

        private Queue<GOAPAction> BuildPlan(GoalComponent goal)
        {
            // Реализуйте логику построения плана действий
            Queue<GOAPAction> plan = new Queue<GOAPAction>();

            if (goal.wantToEat)
            {
                plan.Enqueue(GOAPAction.Eat);
                plan.Enqueue(GOAPAction.FindFood);
            }

            return plan;
        }
    }

    public struct GOAPActionBufferElement : IBufferElementData
    {
        public GOAPAction action;
    }

    public enum GOAPAction
    {
        FindFood,
        Eat
    }
}
