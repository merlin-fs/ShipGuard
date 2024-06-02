using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Reflex.Attributes;

namespace Reflex.Caching
{
    internal static class TypeAttributeInfoCache
    {
        private static readonly ConcurrentDictionary<Type, TypeAttributeInfo> _dictionary = new();
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        private const BindingFlags FlagsStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        internal static TypeAttributeInfo Get(Type type)
        {
            if (!_dictionary.TryGetValue(type, out var info))
            {
                info = Generate(type);
                _dictionary.TryAdd(type, info);
            }
    
            return info;
        }
        
        private static TypeAttributeInfo Generate(Type type)
        {
            var fields = type
                .GetFields(Flags)
                .Concat(type.BaseType?.GetFields(Flags))
                .Where(f => f.IsDefined(typeof(InjectAttribute)))
                .ToArray();

            var fieldsStatic = type
                .GetFields(FlagsStatic)
                .Concat(type.BaseType?.GetFields(FlagsStatic))
                .Where(f => f.IsDefined(typeof(InjectAttribute)))
                .ToArray();
            
            var properties = type
                .GetProperties(Flags)
                .Concat(type.BaseType?.GetProperties(Flags))
                .Where(p => p.CanWrite && p.IsDefined(typeof(InjectAttribute)))
                .ToArray();
            
            var methods = type
                .GetMethods(Flags)
                .Concat(type.BaseType?.GetMethods(Flags))
                .Where(m => m.IsDefined(typeof(InjectAttribute)))
                .ToArray();

            return new TypeAttributeInfo(fields, fieldsStatic, properties, methods);
        }
    }
}