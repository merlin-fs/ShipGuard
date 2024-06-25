using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using Unity.Collections.LowLevel.Unsafe;

using UnityEngine;

namespace Common.Core
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Uuid : IComparable, IComparable<Uuid>, IEquatable<Uuid>
    {
        [FieldOffset(0), SerializeField]
        private long m_FirstHalf;
        [FieldOffset(8), SerializeField]
        private long m_SecondHalf;
        [FieldOffset(0), NonSerialized]
        private readonly Guid m_Guid;
        
        public Guid Guid => m_Guid;
        public unsafe string Name => Encoding.ASCII.GetString(Addr, 16);

        public unsafe static Uuid FromByte(sbyte[] chars)
        {
            Uuid value = default;
            UnsafeUtility.MemCpy(value.Addr, UnsafeUtility.AddressOf(ref chars[0]), 16);
            return value;
        }
        
        private readonly unsafe byte* Addr
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                fixed (void* ptr = &m_FirstHalf)
                    return ((byte*)ptr);
            }
        }
        
        public int CompareTo(object obj)
        {
            return obj is Uuid other
                ? CompareTo(other)
                : GetHashCode() - obj.GetHashCode();
        }

        public unsafe int CompareTo(Uuid other)
        {
            return UnsafeUtility.MemCmp(Addr, other.Addr, 16);
        }

        public bool Equals(Uuid other)
        {
            return CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return obj is Uuid other && Equals(other);
        }

        public override string ToString() => Name;

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public static bool operator == (Uuid lhs, Uuid rhs)
        {
            return lhs.Equals(rhs);
        }
        
        public static bool operator != (Uuid lhs, Uuid rhs)
        {
            return !lhs.Equals(rhs);
        } 
    }
}
