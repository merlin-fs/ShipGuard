using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

using Unity.Entities;

namespace Common.Defs
{
    public static class DefHelper
    {
        private static readonly ConcurrentDictionary<object, ConstructorDefinable> m_Defs = new();

        private record ConstructorDefinable
        {
            private readonly Delegate m_MakeDefinable;
            private readonly object m_Link;

            public ConstructorDefinable(Delegate makeDefinable, object link)
            {
                m_MakeDefinable = makeDefinable;
                m_Link = link;
            }

            public T CreateDefinable<T>()
                where T : unmanaged, IDefinable
            {
                return (T)m_MakeDefinable.DynamicInvoke(m_Link);
            }
        }
        
        public static void AddComponentData<T>(this IDef<T> self, Entity entity, IDefinableContext context)
            where T : unmanaged, IDefinable, IComponentData
        {
            var data = GetConstructorDefinable(self).CreateDefinable<T>();
            if (data is IDefinableCallback callback)
                callback.AddComponentData(entity, context);
            context.AddComponentData(entity, data);
        }

        private static ConstructorDefinable GetConstructorDefinable<T>(IDef<T> self)
            where T : unmanaged, IDefinable, IComponentData
        {
            if (m_Defs.TryGetValue(self, out var rec)) return rec;

            var defType = typeof(RefLink<>).MakeGenericType(self.GetType());
            var link = defType
                .GetMethod("From", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)!
                .Invoke(null, new object[] {self});

            var param = Expression.Parameter(defType, "val");
            var lambda =
                Expression.Lambda(
                    Expression.New(
                        typeof(T).GetConstructor(new[] {defType}) ?? throw new InvalidOperationException(), param),
                    param);
            var makeDefinable = lambda.Compile();
            rec = new ConstructorDefinable(makeDefinable, link);
            m_Defs.TryAdd(self, rec);
            return rec;
        }
        
        public static void RemoveComponentData<T>(this IDef<T> self, Entity entity, T data, IDefinableContext context)
            where T : unmanaged, IDefinable, IComponentData
        {
            if (data is IDefinableCallback callback)
                callback.RemoveComponentData(entity, context);
            context.RemoveComponent<T>(entity);
        }
    }
}