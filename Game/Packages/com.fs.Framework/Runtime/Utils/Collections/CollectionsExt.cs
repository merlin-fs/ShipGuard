using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Common.Core;


namespace System.Collections.Generic
{
    public static class CollectionsExt
    {
        public static T RandomElement<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (!(source is IList<T> list))
                list = source.ToList<T>();
            return list.Count == 0 
                ? default 
                : list[Dice.Range(0, list.Count)];
        }

        public static T RandomElement<T>(this NativeParallelHashSet<T> source)
            where T : unmanaged, IEquatable<T>
        {
            if (source.IsEmpty)
                return default;
            var list = new List<T>();
            foreach(var iter in source)
                list.Add(iter);
            return list.Count == 0
                ? default
                : list[Dice.Range(0, list.Count)];
        }

        public static void Reverse<T>(this NativeList<T> self)
            where T : unmanaged
        {
            var idx1 = 0;
            for (var idx2 = self.Length - 1; idx1 < idx2; --idx2)
            {
                (self[idx2], self[idx1]) = (self[idx1], self[idx2]);
                ++idx1;
            }
        }
    }
}