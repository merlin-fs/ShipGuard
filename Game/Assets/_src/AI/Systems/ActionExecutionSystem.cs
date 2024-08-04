using Unity.Entities;

using UnityEngine;

namespace Game.AI.GOAP
{
    public partial class ActionExecutionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (actionQueue, worldState) in SystemAPI.Query<DynamicBuffer<GOAPActionBufferElement>, RefRW<WorldStateComponent>>())
            {
                if (actionQueue.Length == 0)
                    return;

                GOAPActionBufferElement currentActionElement = actionQueue[0];
                GOAPAction currentAction = currentActionElement.action;

                // Выполнить текущее действие
                bool actionCompleted = ExecuteAction(ref worldState.ValueRW, currentAction);

                if (actionCompleted)
                {
                    // Удалить выполненное действие из очереди
                    actionQueue.RemoveAt(0);
                }
            }
        }

        private bool ExecuteAction(ref WorldStateComponent worldState, GOAPAction action)
        {
            // Реализуйте логику выполнения действия
            switch (action)
            {
                case GOAPAction.FindFood:
                    // Логика поиска еды
                    return true;
                case GOAPAction.Eat:
                    // Логика еды
                    worldState.someState = false; // Например, агент больше не голоден
                    return true;
                default:
                    return false;
            }
        }
    }
}