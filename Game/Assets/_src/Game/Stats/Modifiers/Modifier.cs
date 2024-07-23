using System;
using System.Reflection;
using System.Runtime.InteropServices;

using Game.Core;

using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Properties;
using UnityEngine;

namespace Game.Model.Stats
{
    [Serializable]
    public unsafe struct Modifier : IBufferElementData, IEquatable<Modifier>
    {
        private static readonly MethodInfo m_Method = typeof(IModifier).GetMethod(nameof(IModifier.Estimation));

        public bool Active;
        
        [CreateProperty] private string ID => m_StatId.ToString();
        private Stat.Handle m_StatId;

        //[NativeDisableUnsafePtrRestriction]
        private readonly ulong m_ModifierPtr;
        [HideInInspector]
        public ulong UID => (ulong)m_ModifierPtr;

        private Modifier(void* ptr, Stat.Handle statId)
        {
            m_StatId = statId;
            m_ModifierPtr = (ulong)ptr;
            Active = true;
        }

        public bool HasStat(Stat stat)
        {
            return stat == m_StatId;
        }
        bool IEquatable<Modifier>.Equals(Modifier other)
        {
            return (m_ModifierPtr == other.m_ModifierPtr);
        }

        public static Modifier Create<T, S>(ref T modifier)
            where T : struct, IModifier
            where S : IStat
        {
            UnsafeUtility.PinGCObjectAndGetAddress(modifier, out ulong handle);
            return new Modifier((void*)handle, Stat.Handle.From<S>())
            {
                //TypeIndex = TypeManager.GetTypeIndex<T>(),
            };
        }

        public void Dispose()
        {
            UnsafeUtility.ReleaseGCObject(m_ModifierPtr);
        }

        [BurstDiscard]
        public void Estimation(Entity entity, ref Stat stat, float delta)
        {
            //var obj = (IModifier)Marshal.PtrToStructure(new IntPtr((void*)m_ModifierPtr), TypeManager.GetTypeInfo(TypeIndex).Type);
            var obj = (IModifier)GCHandle.FromIntPtr(new IntPtr((void*)m_ModifierPtr)).Target;
            obj.Estimation(entity, ref stat, delta);
        }

        public static void Estimation(Entity entity, ref Stat stat, in DynamicBuffer<Modifier> items, float delta)
        {
            stat.Reset();
            foreach (var item in items)
                if (item.Active && item.HasStat(stat))
                {
                    item.Estimation(entity, ref stat, delta);
                }
        }

        public static int AddModifier(Modifier modifier, ref DynamicBuffer<Modifier> items)
        {
            var id = FindFreeItem(items);
            if (id < 0)
                id = items.Add(modifier);
            else
                items[id] = modifier;

            return id;

            int FindFreeItem(DynamicBuffer<Modifier> items)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (!items[i].Active)
                        return i;
                }
                return -1;
            }
        }

        public static void DelModifier(ulong uid, ref DynamicBuffer<Modifier> items)
        {
            if (uid == 0)
                return;

            var id = FindFreeItem(items);
            if (id < 0)
                return;

            items[id].Dispose();
            items[id] = new Modifier() { Active = false };

            int FindFreeItem(DynamicBuffer<Modifier> items)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].UID == uid)
                        return i;
                }
                return -1;
            }
        }
    }
}