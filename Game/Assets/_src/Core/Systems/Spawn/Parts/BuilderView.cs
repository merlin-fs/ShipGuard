using System;

using Common.Core;

using Game.Core.Defs;
using Game.Core.HybridTransforms;

using Reflex.Core;

using Unity.Entities;
using Unity.Transforms;

namespace Game.Core.Spawns
{
    public partial class Spawner
    {
        public partial class Builder
        {
            public Builder WithView()
            {
                m_Spawner.m_Ecb.AddComponent<Spawn.ViewTag>(m_Spawner.m_Entity);
                return this;
            }
        }
        
        public Container CreateContext(Entity entity, IConfig config, EntityCommandBuffer ecb, DynamicBuffer<PrefabInfo.BakedInnerPathPrefab> children)
        {
            if (config is not IViewPrefab viewPrefab) 
                throw new NotImplementedException($"IViewPrefab {config.ID} NotImplemented");
                
            var prefab = viewPrefab.GetViewPrefab();
            if (!prefab) throw new ArgumentNullException($"ViewPrefab {config.ID} not assigned");
                
            var newContext = m_Container.Scope(builder =>
            {
                builder.AddSingleton(container =>
                {
                    var inst = m_Factory.Instantiate(prefab, entity, container);
                    return inst;
                });
            });
                
            
            ecb.AddComponent(entity, new HybridTransform.ContainerReference{ Value = newContext });
            ecb.AddComponent<LocalTransform>(entity);
            ecb.AddComponent<LocalToWorld>(entity);
            return newContext;

            //AddChildPrefab(newContext, children);
            //initialization spawn prefab
            //m_Pool.Get().SetContext(newContext);
        }
    }
}
