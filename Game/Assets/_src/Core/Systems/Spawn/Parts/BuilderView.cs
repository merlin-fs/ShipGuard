using System;

using Common.Core;

using Game.Core.Defs;
using Game.Core.HybridTransforms;
using Game.Views;

using Reflex.Core;

using Unity.Entities;
using Unity.Transforms;

namespace Game.Core.Spawns
{
    public partial class Spawner
    {
        public partial class Builder
        {
            public Builder WithNewView()
            {
                m_Ecb.AddComponent<Spawn.ViewTag>(m_Entity);
                //m_Ecb.AddBuffer<PrefabInfo.BakedInnerPathPrefab>(m_Entity);
                return this;
            }

            public Builder WithView(IView view)
            {
                m_Ecb.AddComponent<Spawn.ViewAttachTag>(m_Entity);
                return this;
            }
        }

        public IView CreateView(Entity entity, IConfig config, EntityManager entityManager, EntityCommandBuffer ecb)
        {
            var inst = m_Factory.Instantiate(config, entity, entityManager, m_Container);

            ecb.AddComponent(entity, new HybridTransform.ViewReference{ Value = inst });
            ecb.AddComponent<LocalTransform>(entity);
            ecb.AddComponent<LocalToWorld>(entity);

            return inst;
            
        }
    }
}
