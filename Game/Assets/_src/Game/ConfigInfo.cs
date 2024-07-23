using Common.Core;

using Unity.Entities;

namespace Game.Model
{
    public struct ConfigInfo : IComponentData, IStorageData
    {
        public ObjectID ConfigId;
    }
}
