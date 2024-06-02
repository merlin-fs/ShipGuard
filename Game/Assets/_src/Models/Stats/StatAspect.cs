using System;
using System.Linq;
using System.Collections.Generic;

using Game.Core;

using Unity.Entities;
using Unity.Properties;
using Unity.Collections;

namespace Game.Model.Stats
{
    public readonly partial struct StatAspect : IAspect
    {
        private readonly Entity m_Self;

        private readonly DynamicBuffer<Stat> m_Items;
        [ReadOnly]
        private readonly DynamicBuffer<Modifier> m_Modifiers;

        public Entity Self => m_Self;
        public DynamicBuffer<Stat> Values => m_Items;

        public void Estimation(float delta)
        {
            for (int i = 0; i < m_Items.Length; i++)
            {
                Modifier.Estimation(Self, ref m_Items.ElementAt(i), m_Modifiers, delta);
            }
        }

        public ref Stat GetRW<T>(T stat)
            where T: struct, IConvertible
        {
            return ref m_Items.GetRW(stat);
        }

        public ref Stat GetRW(EnumHandle statId)
        {
            return ref m_Items.GetRW(statId);
        }

        public Stat GetRO<T>(T stat)
            where T: struct, IConvertible
        {
            return m_Items.GetRO(stat);
        }

        public Stat GetRO(EnumHandle statId)
        {
            return m_Items.GetRO(statId);
        }

        #region DesignTime

#if UNITY_EDITOR
        [CreateProperty]
        public readonly List<Stat> StatsNames => m_Items.AsNativeArray().ToArray().ToList();//Select(i => i.StatName)

#endif
        #endregion
    }
}