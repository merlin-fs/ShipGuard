using Common.Core;

using Unity.Entities;

namespace Game.Model
{
    public interface IGameEntity
    {
        Entity Entity { get; }
    }

    public interface IUniqueEntity : IIdentifiable<Uuid>
    {
        
    }
}
