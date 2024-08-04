using System;
using Unity.Collections;
using Unity.Properties;

using UnityEngine;

namespace Common.Core
{
    [Serializable]
    public struct ObjectID : IEquatable<ObjectID>
    {
        [SerializeField, HideInInspector]
        private FixedString64Bytes id;
     
        [CreateProperty] 
        private FixedString64Bytes ID => id;

        private ObjectID(FixedString64Bytes id)
        {
            this.id = id;
        }

        public static ObjectID Create(string value) 
        {
            FixedString64Bytes fs = default;
            fs.CopyFromTruncated(value);
            return new ObjectID(fs);
        }

        public override string ToString()
        {
            return id.ToString();
        }
        public bool Equals(ObjectID other)
        {
            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            var result = (obj is ObjectID other) 
                ? Equals(other) ? 1 : 0 
                : 0;
            return result != 0;
        }

        public static bool operator == (ObjectID left, ObjectID right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ObjectID left, ObjectID right)
        {
            return !left.Equals(right);
        }

        public static implicit operator ObjectID(string value) => Create(value);
        //public static implicit operator string(ObjectID value) => value.ToString();
        public static explicit operator string(ObjectID value) => value.ToString();

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}