using Unity.Entities;

namespace Game.AI.GOAP
{
    public partial class WorldStateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var worldState in SystemAPI.Query<RefRW<WorldStateComponent>>())
            {
                // Логика обновления состояния мира
                worldState.ValueRW.someState = CheckWorldState();
            }
        }

        private bool CheckWorldState()
        {
            // Реализуйте логику проверки состояния мира
            return true;
        }
    }
}
