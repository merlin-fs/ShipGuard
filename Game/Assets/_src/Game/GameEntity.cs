using Common.Core;

using Unity.Entities;
using Unity.Properties;

namespace Game.Model
{
    public struct GameEntity : IGameEntity, IComponentData, IStorageData
    {
        private readonly Uuid m_ID;
        private Entity m_Entity;
        
        public Uuid ID => m_ID;
        [CreateProperty] public Entity Entity => m_Entity;
        [CreateProperty] public string DebugID => m_ID.ToString();
        

        public GameEntity(Uuid id, Entity entity)
        {
            m_Entity = entity;
            m_ID = id;
        }

        public void Initialization(Entity entity)
        {
            m_Entity = entity;
        }
    }
}
