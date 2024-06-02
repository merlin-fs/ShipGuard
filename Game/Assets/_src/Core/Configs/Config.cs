using System;
using Unity.Entities;

namespace Common.Defs
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
        void IConfig.Configure(Entity root, IDefinableContext context)
        {
            m_Prefab = root;
            Configure(m_Prefab, context);
        }

        protected abstract void Configure(Entity root, IDefinableContext context);
    }
}
