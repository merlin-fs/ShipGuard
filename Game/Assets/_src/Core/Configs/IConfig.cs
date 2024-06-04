
using Common.Core;
using Unity.Entities;

namespace Game.Core.Defs
{
    public interface IConfig: IIdentifiable<ObjectID>
    {
        Entity EntityPrefab { get; }
        void Configure(Entity root, IDefinableContext context);
    }
}
