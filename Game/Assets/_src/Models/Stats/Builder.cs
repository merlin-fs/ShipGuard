using System;

using Unity.Entities;

namespace Game.Model.Stats
{
    public partial struct Stat
    {
        public static void AddStat<T>(DynamicBuffer<Stat> buff, float initial)
            where T: IStat
        {
            AddStat<T>(buff, (StatValue)initial);
        }

        public static void AddStat<T>(DynamicBuffer<Stat> buff, StatValue value)
            where T: IStat
        {
            var statId = Handle.From<T>();
            var element = new Stat(statId) {m_Value = value};
            var id = FindStat(buff, statId);
            if (id < 0)
                buff.Add(element);
            else
                buff.ElementAt(id) = element;
        }

        public static bool Has(DynamicBuffer<Stat> buff, Handle statId)
        {
            return FindStat(buff, statId) >= 0;
        }

        public static ref Stat GetRW(DynamicBuffer<Stat> buff, Handle statId)
        {
            var id = FindStat(buff, statId);
            if (id == -1)
                throw new NotImplementedException($"Stat: {statId}");
            return ref buff.ElementAt(id);
        }

        public static Stat GetRO(DynamicBuffer<Stat> buff, Handle statId)
        {
            var id = FindStat(buff, statId);
            if (id == -1)
                throw new NotImplementedException($"Stat: {statId}");
            return buff[id];
        }

        public static bool TryGetStat<T>(DynamicBuffer<Stat> buff, out Stat data)
            where T: IStat
        {
            data = m_Null;
            var id = FindStat(buff, Handle.From<T>());
            if (id == -1)
                return false;
            data = buff[id];
            return true;
        }

        public static void SetStat<T>(DynamicBuffer<Stat> buff, ref Stat data)
            where T: IStat
        {
            var id = FindStat(buff, Handle.From<T>());
            if (id == -1)
                return;

            buff.ElementAt(id) = data;
        }

        private static int FindStat(DynamicBuffer<Stat> buff, Handle statId)
        {
            for (int i = 0; i < buff.Length; i++)
            {
                if (buff[i].m_ID == statId)
                    return i;
            }
            return -1;
        }

        private Stat(Handle value)
        {
            m_ID = value;
            m_Value = StatValue.Default;
        }

        public override bool Equals(object obj) => obj is Stat stat && stat.m_ID == m_ID;
        public override int GetHashCode() => m_ID.GetHashCode();
        public override string ToString() => m_ID.ToString();
        public static bool operator ==(Stat left, Handle right) => left.m_ID == right;
        public static bool operator !=(Stat left, Handle right) => left.m_ID != right;
        public static bool operator ==(Stat left, Stat right) => left.m_ID == right.m_ID;
        public static bool operator !=(Stat left, Stat right) => left.m_ID != right.m_ID;
    }
}
