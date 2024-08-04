using System.Collections.Generic;

using Unity.Entities;

using UnityEngine;

namespace Game.AI.GOAP
{
    public partial class ActionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (action, goal) in SystemAPI.Query<RefRW<ActionComponent>, GoalComponent>())
            {
                // Логика обновления действий на основе цели
                action.ValueRW.isActionAvailable = DetermineAction(goal);
            }
        }

        private bool DetermineAction(GoalComponent goal)
        {
            // Реализуйте логику определения доступных действий
            return goal.wantToEat;
        }
    }
}
