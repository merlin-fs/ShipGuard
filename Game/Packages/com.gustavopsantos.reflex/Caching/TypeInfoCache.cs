using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reflex.Attributes;

using UnityEngine.Pool;

namespace Reflex.Caching
{
    internal static class TypeInfoCache
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        private const BindingFlags FlagsStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        private static readonly ConcurrentDictionary<Type, TypeAttributeInfo> _dictionary = new();
        
        internal static TypeAttributeInfo Get(Type type)
        {
            if (_dictionary.TryGetValue(type, out var info)) return info;
            
            using var pooledFields = ListPool<FieldInfo>.Get(out var fields);
            using var pooledProperties = ListPool<PropertyInfo>.Get(out var properties);
            using var pooledMethods = ListPool<MethodInfo>.Get(out var methods);
            Generate(type, fields, properties, methods);
            info = new TypeAttributeInfo(fields.ToArray(), properties.ToArray(), methods.ToArray());
            _dictionary.TryAdd(type, info);

            return info;
        }
        
        private static void Generate(Type type, List<FieldInfo> fields, List<PropertyInfo> properties, List<MethodInfo> methods)
        {
            var lFields = type
                .GetFields(Flags)
                .Where(f => f.IsDefined(typeof(InjectAttribute)));

            var lFieldsStatic = type
                .GetFields(FlagsStatic)
                .Where(f => f.IsDefined(typeof(InjectAttribute)))
                .ToArray();
            
            var lProperties = type
                .GetProperties(Flags)
                .Where(p => p.CanWrite && p.IsDefined(typeof(InjectAttribute)));

            var lMethods = type
                .GetMethods(Flags)
                .Where(m => m.IsDefined(typeof(InjectAttribute)));
            
            fields.AddRange(lFields);
            fields.AddRange(lFieldsStatic);
            properties.AddRange(lProperties);
            methods.AddRange(lMethods);

            if (type.BaseType != null)
            {
                Generate(type.BaseType, fields, properties, methods);
            }
        }
    }
}