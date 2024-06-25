using System;

using Common.Core;

using Game.Model;
using Game.Views;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Game.Core.Spawns
{
    public partial class Spawner
    {
        public partial class Builder
        {
            private readonly Entity m_Entity;
            private EntityCommandBuffer m_Ecb;
            

            public Builder(Spawner spawner, EntityCommandBuffer ecb)
            {
                m_Ecb = ecb;
                m_Entity = m_Ecb.CreateEntity(spawner.m_EntityArchetype); 
            }

            public Builder WithConfigId(ObjectID configId)
            {
                m_Ecb.AddComponent(m_Entity, new ConfigInfo{ConfigId = configId});
                return this;
            }
            
            public Builder WithId(Uuid id)
            {
                m_Ecb.AddComponent(m_Entity, new GameEntity(id, Entity.Null));
                return this;
            }

            public Builder WithPosition(float3 value)
            {
                
                m_Ecb.AddComponent(m_Entity, new LocalTransform{Position = value});
                return this;
            }
            
            public Builder WithEvent(Action<IView> onDone)
            {
                m_Ecb.AddComponent<Spawn.Event>(m_Entity,
                    new Spawn.Event()
                    {
                        Callback = onDone,
                    });
                return this;
            }
        }
    }
}
