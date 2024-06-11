using System.Collections.Generic;

namespace Common.Core
{
    public static class ManagerUuids
    {
        private static Dictionary<string, Generator16ID> m_Generators = new();
        private static Dictionary<Uuid, int> m_Uuids = new();

        public static Uuid GetUid(this Uuid self, string prefix, int objID, out bool change)
        {
            change = false;
            if (self.GetHashCode() == 0)
            {
                change = true;
                var generator = NeedGenerator(prefix);
                var result = Uuid.FromByte(generator.Next);
                m_Uuids.Add(result, objID);
                return result;
            }
            else if (m_Uuids.TryGetValue(self, out var storeObID) && storeObID != objID)
            {
                change = true;
                var generator = NeedGenerator(prefix);
                var result = Uuid.FromByte(generator.Next);
                m_Uuids.Add(result, objID);
                return result;
            }
            m_Uuids[self] = objID;
            return self;
        }

        public static void Remove(this Uuid self)
        {
            m_Uuids.Remove(self);
        }

        private static Generator16ID NeedGenerator(string prefix)
        {
            if (!m_Generators.TryGetValue(prefix, out var generator))
            {
                generator = new Generator16ID(prefix, null);
                m_Generators.Add(prefix, generator);
            }

            return generator;
        }
    }
}