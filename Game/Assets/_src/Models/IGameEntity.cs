using Common.Core;

using Unity.Entities;

namespace Game.Model
{
    public interface IGameEntity : IIdentifiable<Uuid>
    {
        Entity Entity { get; }
    }
}
