using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

using UnityEngine;

namespace Game.Core
{
    public class EnumHandleAttribute: Attribute{}

    public interface ICustomHandle
    {
        int ID { get; }
    }
    
    public readonly partial struct EnumHandle
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialization()
        {
            Manager.Initialize();            
        }
        
        public readonly struct Manager
        {
            private static bool m_IsInit = false;
            private static readonly object m_Look = new object();

            public static EnumHandle GetHandle<T>(T value)
            {
                return SharedEnumHandle<T>.Get(UnsafeUtility.As<T, int>(ref value));
            }

            public static string GetName(EnumHandle value)
            {
                return m_Names[value];
            }

            public static EnumHandle FromEnum(object value)
            {
                var type = value.GetType();
                return type.IsEnum && type.GetCustomAttribute<EnumHandleAttribute>() != null
                    ? SharedEnumHandle.Get(type, (int)value)
                    : throw new TypeAccessException($"{type}");
            }

            private struct EnumHandleManagerKeyContext { }

            private struct SharedEnumHandle<TComponent>
            {
                public static ref EnumHandle Get(int idx) => ref m_Ref.Data.ElementAt(idx);
                static readonly SharedStatic<FixedList64Bytes<EnumHandle>> m_Ref = SharedStatic<FixedList64Bytes<EnumHandle>>.GetOrCreate<EnumHandleManagerKeyContext, TComponent>();
            }

            private struct SharedEnumHandle
            {
                public static ref EnumHandle Get(Type componentType, int idx)
                {
                    return ref SharedStatic<FixedList64Bytes<EnumHandle>>.GetOrCreate(typeof(EnumHandleManagerKeyContext), componentType).Data.ElementAt(idx);
                }
                public static void Set(Type componentType, int idx, EnumHandle value)
                {
                    ref var data = ref SharedStatic<FixedList64Bytes<EnumHandle>>.GetOrCreate(typeof(EnumHandleManagerKeyContext), componentType).Data;
                    if (data.IsEmpty)
                    {
                        data.Length = componentType.GetEnumValues().Length;
                    }
                    data.ElementAt(idx) = value;
                }
            }

            private static readonly Dictionary<EnumHandle, string> m_Names = new Dictionary<EnumHandle, string>();

            private static void RegistryEnum(Type type)
            {
                foreach (var e in type.GetEnumValues())
                {
                    var name = $"{e} ({e.GetType().DeclaringType?.Name})";
                    var stringId = $"{e.GetType().FullName}+{e}";
                    var handle = new EnumHandle(stringId.GetHashCode());
                    SharedEnumHandle.Set(type, (int)e, handle);
                    m_Names.Add(handle, name);
                }
            }

            public static void Initialize()
            {
                lock (m_Look)
                {
                    if (m_IsInit)
                        return;
                    m_IsInit = true;

                    m_Names.Clear();
                    m_Names.Add(new EnumHandle(0), $"null");

                    var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(t => t.IsEnum && t.GetCustomAttribute<EnumHandleAttribute>() != null);
                
                    foreach (var iter in types)
                    {
                        RegistryEnum(iter);
                    }
                }
            }
        }
    }
}