using Common.Core.Views;

using Game.Core.Defs;

using Reflex.Core;

using Unity.Entities;

using UnityEngine;

using IView = Game.Views.IView;

namespace Game.Core.Spawns
{
    public partial struct Spawn
    {
        public interface IViewFactory
        {
            IView Instantiate(IConfig config, Entity entity, EntityManager entityManager, Container container);
        }
    }
}
