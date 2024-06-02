using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

namespace Common.Defs
{
    public class EntityManagerContext: IDefinableContext
    {
        private EntityManager m_Manager;
        private HashSet<BuffKey> m_Buffs;

        public EntityManagerContext(EntityManager manager)
        {
            m_Manager = manager;
            m_Buffs = null;
        }

        private bool HasChache<T>(Entity entity, out DynamicBuffer<T> value)
            where T : unmanaged
        {
            value = default;
            if (m_Buffs == null)
                return false;
            var key = new BuffKey(entity, typeof(T), null);
            if (m_Buffs.TryGetValue(key, out BuffKey found))
            {
                value = found.GetBuffer<T>();
                return true;
            }
            return false;
        }

        private void AddToChache<T>(Entity entity, DynamicBuffer<T> value)
            where T : unmanaged
        {
            m_Buffs ??= new HashSet<BuffKey>();
            m_Buffs.Add(new BuffKey(entity, typeof(T), value));
        }

        public DynamicBuffer<T> AddBuffer<T>(Entity entity) 
            where T : unmanaged, IBufferElementData
        {
            if (!HasChache(entity, out DynamicBuffer<T> result))
            {
                result = m_Manager.AddBuffer<T>(entity);
                AddToChache<T>(entity, result);
            }
            return result;
        }

        public void AppendToBuffer<T>(Entity entity, T data) 
            where T : unmanaged, IBufferElementData
        {
            var buff = m_Manager.GetBuffer<T>(entity);
            buff.Add(data);
        }

        public void AddComponentData<T>(Entity entity, T data) 
            where T : unmanaged, IComponentData
        {
            m_Manager.AddComponentData<T>(entity, data);
        }

        public void RemoveComponent<T>(Entity entity)
            where T : unmanaged, IComponentData
        {
            m_Manager.RemoveComponent<T>(entity);
        }

        public void SetComponentEnabled<T>(Entity entity, bool value) where T : unmanaged, IEnableableComponent
        {
            m_Manager.SetComponentEnabled<T>(entity, value);
        }

        public void SetName(Entity entity, string name)
        {
            FixedString64Bytes fs = default;
            FixedStringMethods.CopyFromTruncated(ref fs, name);
            m_Manager.SetName(entity, fs);
        }

        public Entity CreateEntity()
        {
            return m_Manager.CreateEntity();
        }
    }

    public class CommandBufferContext : IDefinableContext
    {
        private EntityCommandBuffer m_Manager;
        private HashSet<BuffKey> m_Buffs;

        public CommandBufferContext(EntityCommandBuffer manager)
        {
            m_Manager = manager;
            m_Buffs = null;
        }

        private bool HasChache<T>(Entity entity, out DynamicBuffer<T> value)
            where T : unmanaged
        {
            value = default;
            if (m_Buffs == null)
                return false;
            var key = new BuffKey(entity, typeof(T), null);
            if (m_Buffs.TryGetValue(key, out BuffKey found))
            {
                value = found.GetBuffer<T>();
                return true;
            }
            return false;
        }

        private void AddToChache<T>(Entity entity, DynamicBuffer<T> value)
            where T : unmanaged
        {
            m_Buffs ??= new HashSet<BuffKey>();
            m_Buffs.Add(new BuffKey(entity, typeof(T), value));
        }

        public DynamicBuffer<T> AddBuffer<T>(Entity entity)
            where T : unmanaged, IBufferElementData
        {
            if (!HasChache(entity, out DynamicBuffer<T> result))
            {
                result = m_Manager.AddBuffer<T>(entity);
                AddToChache<T>(entity, result);
            }
            return result;
        }

        public void AppendToBuffer<T>(Entity entity, T data)
            where T : unmanaged, IBufferElementData
        {
            m_Manager.AppendToBuffer<T>(entity, data);
        }

        public void AddComponentData<T>(Entity entity, T data)
            where T : unmanaged, IComponentData
        {
            m_Manager.AddComponent<T>(entity, data);
        }

        public void RemoveComponent<T>(Entity entity)
            where T : unmanaged, IComponentData
        {
            m_Manager.RemoveComponent<T>(entity);
        }

        public void SetComponentEnabled<T>(Entity entity, bool value) where T : unmanaged, IEnableableComponent
        {
            m_Manager.SetComponentEnabled<T>(entity, value);
        }

        public void SetName(Entity entity, string name)
        {
            FixedString64Bytes fs = default;
            FixedStringMethods.CopyFromTruncated(ref fs, name);
            m_Manager.SetName(entity, fs);
        }

        public Entity CreateEntity()
        {
            return m_Manager.CreateEntity();
        }
    }

    public class WriterContext : IDefinableContext
    {
        private EntityCommandBuffer.ParallelWriter m_Manager;
        private readonly int m_SortKey;
        private HashSet<BuffKey> m_Buffs;

        public WriterContext(EntityCommandBuffer.ParallelWriter manager, int sortKey)
        {
            m_Manager = manager;
            m_SortKey = sortKey;
            m_Buffs = null;
        }

        private bool HasChache<T>(Entity entity, out DynamicBuffer<T> value)
            where T : unmanaged
        {
            value = default;
            if (m_Buffs == null)
                return false;
            var key = new BuffKey(entity, typeof(T), null);
            if (m_Buffs.TryGetValue(key, out BuffKey found))
            {
                value = found.GetBuffer<T>();
                return true;
            }
            return false;
        }

        private void AddToChache<T>(Entity entity, DynamicBuffer<T> value)
            where T : unmanaged
        {
            m_Buffs ??= new HashSet<BuffKey>();
            m_Buffs.Add(new BuffKey(entity, typeof(T), value));
        }

        public DynamicBuffer<T> AddBuffer<T>(Entity entity)
            where T : unmanaged, IBufferElementData
        {
            if (!HasChache(entity, out DynamicBuffer<T> result))
            {
                result = m_Manager.AddBuffer<T>(m_SortKey, entity);
                AddToChache<T>(entity, result);
            }
            return result;
        }

        public void AppendToBuffer<T>(Entity entity, T data)
            where T : unmanaged, IBufferElementData
        {
            m_Manager.AppendToBuffer<T>(m_SortKey, entity, data);
        }

        public void AddComponentData<T>(Entity entity, T data)
            where T : unmanaged, IComponentData
        {
            m_Manager.AddComponent<T>(m_SortKey, entity, data);
        }

        public void RemoveComponent<T>(Entity entity)
            where T : unmanaged, IComponentData
        {
            m_Manager.RemoveComponent<T>(m_SortKey, entity);
        }

        public void SetComponentEnabled<T>(Entity entity, bool value) where T : unmanaged, IEnableableComponent
        {
            m_Manager.SetComponentEnabled<T>(m_SortKey, entity, value);

        }

        public void SetName(Entity entity, string name)
        {
            FixedString64Bytes fs = default;
            FixedStringMethods.CopyFromTruncated(ref fs, name);
            m_Manager.SetName(m_SortKey, entity, fs);
        }

        public Entity CreateEntity()
        {
            return m_Manager.CreateEntity(m_SortKey);
        }
    }
    
    struct BuffKey: IEquatable<BuffKey>
    {
        Entity m_Entity;
        TypeIndex m_TypeIndex;
        object m_Buff;

        public BuffKey(Entity entity, Type type, object buff)
        {
            m_Entity = entity;
            m_TypeIndex = TypeManager.GetTypeIndex(type);
            m_Buff = buff;
        }

        public DynamicBuffer<T> GetBuffer<T>()
            where T : unmanaged

        {
            return (DynamicBuffer<T>)m_Buff;
        }

        public bool Equals(BuffKey other)
        {
            return other.m_TypeIndex == m_TypeIndex && other.m_Entity == m_Entity;
        }

        public override int GetHashCode()
        {
            return m_TypeIndex;
        }

        public override bool Equals(object obj)
        {
            int result = 0;
            if (obj is BuffKey key)
            {
                result = (Equals(key) ? 1 : 0);
            }
            return (byte)result != 0;
        }
    }
}