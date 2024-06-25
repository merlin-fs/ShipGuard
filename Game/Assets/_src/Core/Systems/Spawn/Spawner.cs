using Game.Core.Defs;

using Reflex.Attributes;
using Reflex.Core;

using Unity.Entities;

namespace Game.Core.Spawns
{
    public partial class Spawner
    {
        [Inject] private Container m_Container;
        private readonly Spawn.IViewFactory m_Factory;
        
        private readonly EntityArchetype m_EntityArchetype;

        public Builder Setup(EntityCommandBuffer ecb)
        {
            return new Builder(this, ecb);
        }
        
        /*
        public Builder Spawn(IConfig config, EntityCommandBuffer ecb)
        {
            m_Config = config;
            m_Ecb = ecb;
            m_Entity = CreateEntity();
            Debug.Log($"[Spawner] spawn: {config.ID} ({m_Entity})");
            return m_Builder;
        }
        */

        /*
        public Builder Spawn(Entity entity, IConfig config, EntityCommandBuffer ecb)
        {
            m_Config = config;
            m_Ecb = ecb;
            m_Entity = entity;
            
            config.Configure(m_Entity, new CommandBufferContext(ecb));
            
            Debug.Log($"[Spawner] spawn: {config.ID} ({m_Entity})");
            return m_Builder;
        }
        */
        
        public Spawner(Spawn.IViewFactory viewFactory)
        {
            m_Factory = viewFactory;
            m_EntityArchetype = World.DefaultGameObjectInjectionWorld.EntityManager
                .CreateArchetype(
                    typeof(Spawn)
                    );
        }
    }
}
