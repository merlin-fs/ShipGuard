using System;

using Game.Core;

using Unity.Entities;
using Unity.Properties;

namespace Game.Model.Stats
{
    public partial struct Stat : IBufferElementData
    {
        private static Stat m_Null = new Stat() { m_ID = Handle.Null, };
        [CreateProperty] private string ID => m_ID.ToString();
        
        private Handle m_ID;
        private StatValue m_Value;

        [CreateProperty] 
        public float Value => m_Value.Current.Value;
        public float Normalize => m_Value.Current.Value / m_Value.Current.Max;

        public void ModMull(float value)
        {
            m_Value.Current.Value *= value;
        }

        public void Damage(float value)
        {
            m_Value.Current.Value -= value;
            m_Value.Original.Value = m_Value.Current.Value;
        }

        public void Reset()
        {
            m_Value.Current = m_Value.Original;
        }
    }
}