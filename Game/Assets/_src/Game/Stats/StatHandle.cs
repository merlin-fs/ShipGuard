using System;

using Game.Core;

namespace Game.Model.Stats
{
    public partial struct Stat
    {
        public struct Handle : ICustomHandle
        {
            public int ID { get; }

            static Handle()
            {
                CustomHandleManager<Handle>.Initialize((type, args) =>
                {
                    var name = $"{type}";
                    var stringId = $"{type.FullName}";
                    var handle = new Handle(stringId.GetHashCode());
                    CustomHandleManager<Handle>.Registry(type, handle, name);
                });
            }

            public static Handle Null { get; } = new Handle(0);

            public static Handle From<T>()
                where T : IStat
            {
                return CustomHandleManager<Handle>.GetHandle<T>();
            }

            public static Handle FromType(Type type)
            {
                return CustomHandleManager<Handle>.GetHandle(type);
            }

            public static void Registry(Type type) => CustomHandleManager<Handle>.Registry(type);

            public static void Registry<T>()
                where T : IStat => Registry(typeof(T));

            private Handle(int id) => ID = id;

            public static bool Equals(Handle l1, Handle l2) => l1.ID == l2.ID;

            public override string ToString() => CustomHandleManager<Handle>.GetName(this);

            public bool Equals(Handle other) => ID == other.ID;

            public override bool Equals(object obj) => obj is Handle lh && Equals(lh);

            public override int GetHashCode() => ID;

            public static bool operator ==(Handle left, Handle right) => left.Equals(right);

            public static bool operator !=(Handle left, Handle right) => !left.Equals(right);

            public int CompareTo(Handle other) => ID - other.ID;

        }
    }
}
