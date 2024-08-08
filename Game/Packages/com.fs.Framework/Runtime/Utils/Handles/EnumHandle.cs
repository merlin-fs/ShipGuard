using System;

namespace Game.Core
{
    public readonly partial struct EnumHandle: IEquatable<EnumHandle>, IComparable<EnumHandle>, ICustomHandle
    {
        public int ID { get => m_ID; }
        private readonly int m_ID;
        public static EnumHandle Null { get; } = new EnumHandle(0);

        public static EnumHandle FromEnum<T>(T value)
            where T : struct, IConvertible
        {
            return Manager.GetHandle(value);
        }

        public static EnumHandle FromObjectEnum(object value)
        {
            return Manager.FromEnum(value);
        }

        public EnumHandle(int id) => m_ID = id;   

        public static bool Equals(EnumHandle l1, EnumHandle l2) => l1.m_ID == l2.m_ID;

        public override string ToString() => Manager.GetName(this);
        
        public bool Equals(EnumHandle other) => m_ID == other.m_ID;

        public override bool Equals(object obj) => obj is EnumHandle lh && Equals(lh);

        public override int GetHashCode() => m_ID;

        public static bool operator ==(EnumHandle left, EnumHandle right) => left.Equals(right);

        public static bool operator !=(EnumHandle left, EnumHandle right) => !left.Equals(right);

        public int CompareTo(EnumHandle other) => m_ID - other.m_ID;
    }
}