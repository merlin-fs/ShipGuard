using System;

using Game.Core;

using Unity.Entities;

namespace Game.AI.GOAP
{
    public readonly struct LogicActionHandle: IEquatable<LogicActionHandle>, IComparable<LogicActionHandle>, 
        IBufferElementData, ICustomHandle
    {
        public int ID { get; }

        static LogicActionHandle()
        {
            CustomHandleManager<LogicActionHandle>.Initialize((type, args) =>
            {
                var name = $"{type}";
                var stringId = $"{type.FullName}";
                var handle = new LogicActionHandle(stringId.GetHashCode());
                CustomHandleManager<LogicActionHandle>.Registry(type, handle, name);
            });
        }
        
        public static LogicActionHandle Null { get; } = new LogicActionHandle(0);

        public static LogicActionHandle From<T>()
            where T : Logic.IAction
        {
            return CustomHandleManager<LogicActionHandle>.GetHandle<T>();
        }

        public static LogicActionHandle FromType(Type type)
        {
            return CustomHandleManager<LogicActionHandle>.GetHandle(type);
        }

        public static void Registry(Type type) => CustomHandleManager<LogicActionHandle>.Registry(type); 
        public static void Registry<T>() where T : Logic.IAction => Registry(typeof(T));
        public LogicActionHandle(int id) => ID = id;

        public static bool Equals(LogicActionHandle l1, LogicActionHandle l2) => l1.ID == l2.ID;

        public override string ToString() => CustomHandleManager<LogicActionHandle>.GetName(this);
        
        public bool Equals(LogicActionHandle other) => ID == other.ID;

        public override bool Equals(object obj) => obj is LogicActionHandle lh && Equals(lh);

        public override int GetHashCode() => ID;

        public static bool operator ==(LogicActionHandle left, LogicActionHandle right) => left.Equals(right);

        public static bool operator !=(LogicActionHandle left, LogicActionHandle right) => !left.Equals(right);

        public int CompareTo(LogicActionHandle other) => ID - other.ID;
    }
}
