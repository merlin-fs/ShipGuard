using Common.Core;

using Unity.Entities;

namespace Game.Model.Locations
{
    public interface ILocationItem : IStorageData {}

    public struct LocationTag : IComponentData {}

    public struct LocationLink : IComponentData, IStorageData
    {
        public Uuid LocationId;
    }
}
