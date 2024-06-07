using System;

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
            private readonly Spawner m_Spawner;
            public Entity Entity => m_Spawner.m_Entity;

            public Builder(Spawner spawner)
            {
                m_Spawner = spawner;
            }

            public Builder WithComponents(DynamicBuffer<Spawn.Component> components)
            {
                foreach (var iter in components)
                    m_Spawner.m_Ecb.AddComponent(m_Spawner.m_Entity, iter.ComponentType);
                return this;
            }

            public Builder WithPosition(float3 value)
            {
                m_Spawner.m_Ecb.AddComponent<LocalTransform>(m_Spawner.m_Entity,
                    new LocalTransform() 
                    {
                        Position = value,
                    });
                return this;
            }
            
            public Builder WithEvent(Action<IView> onDone)
            {
                m_Spawner.m_Ecb.AddComponent<Spawn.Event>(m_Spawner.m_Entity,
                    new Spawn.Event()
                    {
                        Callback = onDone,
                    });
                return this;
            }
            
            
            public Builder WithComponent(object component, Type type)
            {
                m_Spawner.m_Ecb.AddComponent(m_Spawner.m_Entity, component, type);
                return this;
            }

            public Builder WithComponent<T>()
                where T : unmanaged, IComponentData
            {
                m_Spawner.m_Ecb.AddComponent<T>(m_Spawner.m_Entity);
                return this;
            }

            public Builder WithComponent<T>(T component)
                where T : unmanaged, IComponentData
            {
                m_Spawner.m_Ecb.AddComponent<T>(m_Spawner.m_Entity, component);
                return this;
            }
        }
    }
}
