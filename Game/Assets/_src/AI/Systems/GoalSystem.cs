using Game.AI.GOAP;

using Unity.Entities;

namespace Game.AI.GOAP
{
    public partial class GoalSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (goal, worldState) in SystemAPI.Query<RefRW<GoalComponent>, WorldStateComponent>())
            {
                // Логика определения целей на основе состояния мира
                goal.ValueRW.wantToEat = DetermineGoal(worldState);
            };
        }

        private bool DetermineGoal(WorldStateComponent worldState)
        {
            // Реализуйте логику определения цели
            return worldState.someState;
        }
    }
}
