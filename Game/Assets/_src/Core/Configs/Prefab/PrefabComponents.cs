using Unity.Collections;
using Unity.Entities;


namespace Game.Core.Defs
{
    public partial struct PrefabInfo
    {
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
    }
}