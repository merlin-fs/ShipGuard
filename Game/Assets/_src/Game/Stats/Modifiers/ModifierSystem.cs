using System;
using System.Collections.Concurrent;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Game.Model.Stats
{
    [UpdateInGroup(typeof(GameLogicInitSystemGroup))]
    public partial class ModifiersSystem : SystemBase
    {
        public static ModifiersSystem Instance { get; private set; }
        ConcurrentQueue<Item> m_Queue;

        private BufferLookup<Modifier> m_LookupModifiers;

        private struct Item
        {
            public ulong UID;
            public Entity Entity;
            public Modifier Modifier;
        }

        protected override void OnCreate()
        {
            Instance = this;
            m_Queue = new ConcurrentQueue<Item>();
            m_LookupModifiers = GetBufferLookup<Modifier>(false);
        }

        public ulong AddModifier<T, TS>(Entity entity, ref T modifier)
            where T: struct, IModifier
            where TS: IStat
        {
            var mod = Modifier.Create<T, TS>(ref modifier);
            var item = new Item()
            {
                UID = 0,
                Modifier = mod,
                Entity = entity,
            };
            m_Queue.Enqueue(item);
            return mod.UID;
        }

        public void DelModifier(Entity entity, ulong uid)
        {
            var item = new Item()
            {
                UID = uid,
                Modifier = default,
                Entity = entity,
            };
            m_Queue.Enqueue(item);
        }

        protected override void OnUpdate()
        {
            if (m_Queue.Count == 0)
                return;
            
            m_LookupModifiers.Update(this);
            while (m_Queue.Count > 0)
            {
                if (!m_Queue.TryDequeue(out var iter)) break;
                
                if (!m_LookupModifiers.HasBuffer(iter.Entity)) continue;
                var modifiers = m_LookupModifiers[iter.Entity];
                if (iter.UID == 0)
                {
                    Modifier.AddModifier(iter.Modifier, ref modifiers);
                }
                else
                {
                    Modifier.DelModifier(iter.UID, ref modifiers);
                }
            }
        }
    }
}