using Common.Core.Views;

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
            IView Instantiate(GameObject prefab, Entity entity, Container container);
        }
    }
}
