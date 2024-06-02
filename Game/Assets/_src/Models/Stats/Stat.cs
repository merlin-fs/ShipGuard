using System;

using Game.Core;

using Unity.Entities;
using Unity.Properties;

namespace Game.Model.Stats
{
    public partial struct Stat : IBufferElementData
    {
        private static Stat m_Null = new Stat() { m_StatID = EnumHandle.Null, };

        [CreateProperty] private string ID => m_StatID.ToString();
        
        private EnumHandle m_StatID;
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

        public static void AddStat<T>(DynamicBuffer<Stat> buff, T value, float initial)
            where T: struct, IConvertible
        {
            AddStat(buff, value, (StatValue)initial);
        }

        public static void AddStat<T>(DynamicBuffer<Stat> buff, T value, StatValue stat)
            where T: struct, IConvertible
        {
            var element = Stat.FromEnum(value, stat);
            var id = FindStat(buff, value);
            if (id < 0)
                buff.Add(element);
            else
                buff.ElementAt(id) = element;
        }

        public static bool Has(DynamicBuffer<Stat> buff, EnumHandle statId)
        {
            return FindStat(buff, statId) >= 0;
        }

        public static ref Stat GetRW(DynamicBuffer<Stat> buff, EnumHandle statId)
        {
            var id = FindStat(buff, statId);
            if (id == -1)
                throw new NotImplementedException($"Stat: {statId}");
            return ref buff.ElementAt(id);
        }

        public static Stat GetRO(DynamicBuffer<Stat> buff, EnumHandle statId)
        {
            var id = FindStat(buff, statId);
            if (id == -1)
                throw new NotImplementedException($"Stat: {statId}");
            return buff[id];
        }

        public static bool TryGetStat<T>(DynamicBuffer<Stat> buff, T stat, out Stat data)
            where T: struct, IConvertible
        {
            data = m_Null;
            var id = FindStat(buff, stat);
            if (id == -1)
                return false;
            data = buff[id];
            return true;
        }

        public static void SetStat<T>(DynamicBuffer<Stat> buff, T stat, ref Stat data)
            where T: struct, IConvertible
        {
            var id = FindStat(buff, stat);
            if (id == -1)
                return;

            buff.ElementAt(id) = data;
        }

        private static int FindStat<T>(DynamicBuffer<Stat> buff, T stat)
            where T: struct, IConvertible
        {
            return FindStat(buff, EnumHandle.FromEnum(stat));
        }

        private static int FindStat(DynamicBuffer<Stat> buff, EnumHandle statId)
        {
            for (int i = 0; i < buff.Length; i++)
            {
                if (buff[i].m_StatID == statId)
                    return i;
            }
            return -1;
        }

        static Stat FromEnum<T>(T value, StatValue stat)
            where T: struct, IConvertible
        {
            return new Stat(EnumHandle.FromEnum(value)) 
            {
                m_Value = stat
            };
        }

        private Stat(EnumHandle value)
        {
            m_StatID = value;
            m_Value = StatValue.Default;
        }

        public override bool Equals(object obj) => obj is Stat stat && stat.m_StatID == m_StatID;
        public override int GetHashCode() => m_StatID.GetHashCode();
        public override string ToString() => m_StatID.ToString();
        public static bool operator ==(Stat left, EnumHandle right) => left.m_StatID == right;
        public static bool operator !=(Stat left, EnumHandle right) => left.m_StatID != right;
        public static bool operator ==(Stat left, Stat right) => left.m_StatID == right.m_StatID;
        public static bool operator !=(Stat left, Stat right) => left.m_StatID != right.m_StatID;
    }

    public static class StatExt
    {
        public static void AddStat<T>(this DynamicBuffer<Stat> buff, T value, float initial)
            where T: struct, IConvertible
        {
            Stat.AddStat(buff, value, initial);
        }

        public static void AddStat<T>(this DynamicBuffer<Stat> buff, T value, StatValue stat)
            where T: struct, IConvertible
        {
            Stat.AddStat(buff, value, stat);
        }

        public static void AddStat<T>(this DynamicBuffer<Stat> buff, T value, IStatValue stat)
            where T: struct, IConvertible
        {
            Stat.AddStat(buff, value, stat.Value);
        }

        public static bool Has<T>(this DynamicBuffer<Stat> buff, T stat)
            where T: struct, IConvertible
        {
            return Stat.Has(buff, EnumHandle.FromEnum(stat));
        }

        public static bool TryGetStat<T>(this DynamicBuffer<Stat> buff, T stat, out Stat data)
            where T: struct, IConvertible
        {
            return Stat.TryGetStat(buff, stat, out data);
        }

        public static ref Stat GetRW<T>(this DynamicBuffer<Stat> buff, T stat)
            where T: struct, IConvertible
        {
            return ref Stat.GetRW(buff, EnumHandle.FromEnum(stat));
        }

        public static Stat GetRO<T>(this DynamicBuffer<Stat> buff, T stat)
            where T: struct, IConvertible
        {
            return Stat.GetRO(buff, EnumHandle.FromEnum(stat));
        }

        public static ref Stat GetRW(this DynamicBuffer<Stat> buff, EnumHandle statId)
        {
            return ref Stat.GetRW(buff, statId);
        }

        public static Stat GetRO(this DynamicBuffer<Stat> buff, EnumHandle statId)
        {
            return Stat.GetRO(buff, statId);
        }
    }
}