using System;
using System.Collections.Concurrent;

using Unity.Burst;

using UnityEngine;

namespace Game.Core
{
    public readonly struct Manager<THandle>
        where THandle: unmanaged, ICustomHandle
    {
        public delegate void TRegistry(Type type, params object[] args); 
        
        private static TRegistry m_Registry;

        public static void Initialize(TRegistry registry)
        {
            m_Registry = registry;
        }
        
        public static THandle GetHandle<T>(params object[] args)
        {
            return SharedCustomHandle<T>.Get(args);
        }

        public static THandle GetHandle(Type type)
        {
            return SharedCustomHandle.Get(type);
        }

        public static string GetName(THandle value)
        {
            if (m_Names == null) return "names null";
            if (value.ID == 0) return "null";
            m_Names.TryGetValue(value, out var str);
            return str;
        }

        private struct CustomHandleManagerKeyContext
        {
        }

        private struct SharedCustomHandle<TComponent>
        {
            public static ref THandle Get(params object[] args)
            {
                if (m_Ref.Data.ID == 0)
                {
                    m_Registry(typeof(TComponent));
                }
                return ref m_Ref.Data;
            }

            static readonly SharedStatic<THandle> m_Ref = SharedStatic<THandle>
                .GetOrCreate<CustomHandleManagerKeyContext, TComponent>();
        }

        private struct SharedCustomHandle
        {
            public static ref THandle Get(Type componentType)
            {
                ref var data = ref SharedStatic<THandle>
                    .GetOrCreate(typeof(CustomHandleManagerKeyContext), componentType).Data;

                if (data.ID == 0)
                {
                    m_Registry(componentType);
                }
                return ref data;
            }

            public static void Set(Type componentType, THandle value)
            {
                ref var data = ref SharedStatic<THandle>
                    .GetOrCreate(typeof(CustomHandleManagerKeyContext), componentType).Data;
                data = value;
            }
        }

        public static void Registry(Type type, THandle handle, string name)
        {
            SharedCustomHandle.Set(type, handle);
            try
            {
                m_Names.TryAdd(handle, name);
            }
            catch (Exception e)
            {
                Debug.LogError($"[{typeof(THandle)}] {name} : {e}");
            }
        }
        
        public static void Registry(Type type) => m_Registry(type);
        
        private static readonly ConcurrentDictionary<THandle, string> m_Names = new();
    }
}