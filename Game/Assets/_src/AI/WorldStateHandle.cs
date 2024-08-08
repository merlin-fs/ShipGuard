using System;

using Game.Core;

namespace Game.AI.GOAP
{
    public readonly struct WorldStateHandle : IEquatable<WorldStateHandle>, IComparable<WorldStateHandle>
    {
        private readonly EnumHandle m_Handle;
        private readonly int m_ID;
        private readonly bool m_Value;
        public bool Value => m_Value;
        public EnumHandle Enum => m_Handle;

        public static WorldStateHandle Null { get; } = new (EnumHandle.Null, 0, false);

        public static WorldStateHandle FromHandle(EnumHandle handle, bool value)
        {
            return new WorldStateHandle(handle, HashCode.Combine(handle, value), value);
        }

        public static WorldStateHandle FromEnum<T>(T state, bool value)
            where T : struct, IConvertible
        {
            var handle = EnumHandle.FromEnum(state);
            return FromHandle(handle, value);
        }

        private WorldStateHandle(EnumHandle handle, int id, bool value)
        {
            m_Handle = handle;
            m_ID = id;
            m_Value = value;
        }

        public override string ToString()
        {
            return $"{m_Handle} => {m_Value}";
        }

        public bool Equals(WorldStateHandle other)
        {
            return m_ID == other.m_ID;
        }
        
        public override bool Equals(object obj)
        {
            return obj is WorldStateHandle lh && Equals(lh);
        }

        public override int GetHashCode()
        {
            return m_ID.GetHashCode();
        }

        public static bool operator ==(WorldStateHandle left, WorldStateHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WorldStateHandle left, WorldStateHandle right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(WorldStateHandle other)
        {
            return m_ID.CompareTo(other.m_ID);
        }
    }
}
