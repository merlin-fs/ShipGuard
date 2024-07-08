using Common.Core;

using Game.Views;

using Unity.Entities;

namespace Game.Core.Defs
{
    using Core;

    public abstract class Config : IConfig, IIdentifiable<ObjectID>
    {
        private ObjectID m_ID;
        private Entity m_Prefab;

        public ObjectID ID => m_ID;
        public Entity EntityPrefab => m_Prefab;

        protected Config(ObjectID id)
        {
            m_ID = id;
        }
        void IConfig.Configure(Entity entity, EntityManager manager, IDefinableContext context)
        {
            m_Prefab = entity;
            Configure(m_Prefab, manager, context);
        }

        protected abstract void Configure(Entity root, EntityManager manager, IDefinableContext context);
        public abstract ComponentTypeSet GetComponentTypeSet();
        public virtual void Configure(IView view, Entity entity, EntityManager manager) { }
    }
}
