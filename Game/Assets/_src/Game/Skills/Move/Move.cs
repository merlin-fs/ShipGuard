using System;

using Game.Core;

using Unity.Entities;
using Unity.Mathematics;

namespace Game.Model
{
    public partial struct Move : IComponentData
    {
        public float3 Position;
        public quaternion Rotation;
        public float Speed;
        public float Travel;
        public Entity Target;
        public QueryFlags Query;

        public struct InternalData : IComponentData
        {
            public float3 Store; 
            public float Time;
        }
        
        [EnumHandle]
        public enum State
        {
            Init,
            MoveDone,
            PathFound,
        }
        
        [Flags]
        public enum QueryFlags
        {
            None   = 0,
            Move = 1 << 0,
            Rotate = 1 << 1,
            Target = 1 << 2,
        }
    }
}
