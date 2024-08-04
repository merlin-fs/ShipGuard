using Unity.Entities;

namespace Game.AI.GOAP
{
    public struct WorldStateComponent : IComponentData
    {
        public bool someState; // Например, голоден ли агент
    }
}
