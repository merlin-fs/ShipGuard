using System;

using Common.Core;

using Game.Core.Defs;

using Reflex.Attributes;
using Reflex.Core;

using Unity.Entities;

using UnityEngine;

namespace Game.Core.Spawns
{
    public partial class Spawner
    {
        [Inject] private Container m_Container;
        private Spawn.IViewFactory m_Factory;
        
        private IConfig m_Config;
        private EntityCommandBuffer m_Ecb;
        private Entity m_Entity;

        private Builder m_Builder; 

        public Builder Spawn(IConfig config, EntityCommandBuffer ecb)
        {
            m_Config = config;
            m_Ecb = ecb;
            m_Entity = CreateEntity();
            m_Builder.WithComponent<Spawn.Tag>();

            Debug.Log($"[Spawner] spawn: {config.ID} ({m_Entity})");
            return m_Builder;
        }

        public Spawner(Spawn.IViewFactory viewFactory)
        {
            m_Factory = viewFactory;
            m_Builder = new Builder(this);
        }
        
        /*
        private Spawner(IConfig config, EntityCommandBuffer ecb)
        {
            m_Config = config;
            m_Ecb = ecb;
            if (m_Config.EntityPrefab == Entity.Null) 
                throw new ArgumentNullException($"EntityPrefab {m_Config.ID} not assigned");
        }
        */

        private Entity CreateEntity()
        {
            var entity = m_Ecb.Instantiate(m_Config.EntityPrefab);
            m_Ecb.AddComponent(entity, new PrefabInfo{ConfigID = m_Config.ID});
            return entity;
        }
    }
}
