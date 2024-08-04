using Unity.Entities;
using Unity.Properties;

namespace Game.Model
{
    public struct GameEntity : IGameEntity, IComponentData, IStorageData
    {
        private Entity m_Entity;
        [CreateProperty] public Entity Entity => m_Entity;

        public void Initialization(Entity entity)
        {
            m_Entity = entity;
        }
    }
}
