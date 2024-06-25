using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

using Common.Defs;

using Unity.Entities;

namespace Game.Core.Defs
{
    public static class DefHelper
    {
        private static readonly ConcurrentDictionary<object, ConstructorDefinable> m_Defs = new();
        private delegate void SetDef<T>(RefLink<T> link);

        private record ConstructorDefinable
        {
            private MethodInfo m_MethodInfo;
            private readonly object m_Link;

            public ConstructorDefinable(MethodInfo methodInfo, object link)
            {
                m_MethodInfo = methodInfo;
                m_Link = link;
            }

            public T FillDefinable<T>()
                where T : unmanaged, IDefinable
            {
                var value = default(T);
                m_MethodInfo.Invoke(value, new object[]{m_Link});
                return value;
            }
        }
        
        public static void AddComponentData<T>(this IDef<T> self, Entity entity, IDefinableContext context)
            where T : unmanaged, IDefinable, IComponentData
        {
            var data = GetConstructorDefinable(self).FillDefinable<T>();
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

            var methodInfo = typeof(T).GetMethod(nameof(IDefinable<IDef<T>>.SetDef));
            rec = new ConstructorDefinable(methodInfo, link);
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