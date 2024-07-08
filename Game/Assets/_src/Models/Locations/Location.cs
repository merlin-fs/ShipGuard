using Common.Core;

using Unity.Entities;

namespace Game.Model.Locations
{
    public struct LocationTag : IComponentData {}

    public struct LocationLink : IComponentData
    {
        public Uuid LocationId;
    }
}
