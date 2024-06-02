using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Common.Defs
{
    public struct RefLink<T>
    {
        private GCHandle m_RefHandle;
        public T Value => (T)m_RefHandle.Target;
        private RefLink(GCHandle handle) => m_RefHandle = handle;
        public static RefLink<T> Copy<TT>(RefLink<TT> link) => new(link.m_RefHandle);
        public unsafe static RefLink<T> From(T value)
        {
            UnsafeUtility.PinGCObjectAndGetAddress(value, out var gsHandle);
            return new RefLink<T>(GCHandle.FromIntPtr(new IntPtr((void*)gsHandle)));
        }

        public bool IsValid => m_RefHandle.IsAllocated;
        
        public void Free() => m_RefHandle.Free();
    }
}