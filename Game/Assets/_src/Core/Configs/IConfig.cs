
using Common.Core;

using Game.Views;

using Unity.Entities;

namespace Game.Core.Defs
{
    public interface IConfig: IIdentifiable<ObjectID>
    {
        Entity EntityPrefab { get; }
        void Configure(Entity root, EntityManager manager, IDefinableContext context);
        ComponentTypeSet GetComponentTypeSet();
        void Configure(IView view, Entity entity, EntityManager manager);
    }
}
