using Unity.Entities;

namespace Game.AI.GOAP
{
    public struct ActionComponent : IComponentData
    {
        public bool isActionAvailable; // Например, доступно ли действие
        public float cost; // Стоимость действия
    }
}
