using System;

using Common.Defs;

using Unity.Entities;

using UnityEngine;

namespace Game.Core.Defs
{
    public abstract class GameObjectConfigWithDef : GameObjectConfig
    {
        public abstract Type DefType { get; }
    }
    
    public abstract class GameObjectConfigWithDef<T> : GameObjectConfigWithDef
        where T : IDef, new()
    {
        [SerializeField]
        private T value = new ();
        public T Value => value;
        public override Type DefType => Value.GetTypeIndexDefinable().GetManagedType();
        public override ComponentTypeSet GetComponentTypeSet() => new (ComponentType.FromTypeIndex(Value.GetTypeIndexDefinable()));
    }
}
