using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

using Reflex.Core;


namespace Game.Core.Prefabs
{
    public partial struct PrefabInfo
    {
        public struct BakedTag : IComponentData {}

        public class ContextReference : IComponentData
        {
            public Container Value;
        }

        public struct BakedInnerPathPrefab : IBufferElementData
        {
            public FixedString128Bytes Path;
            public Entity Entity;

            public BakedInnerPathPrefab(Entity entity, string path)
            {
                Entity = entity;
                Path = new FixedString128Bytes(path);
            }
        }

        public struct BakedEnvironment : IComponentData
        {
            public int2 Size;
            public float3 Pivot;
            public FixedString128Bytes Layer;
        }
    }
}