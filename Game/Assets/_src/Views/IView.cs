using System.Collections.Generic;

using Common.Core;

using Game.Model;

using Unity.Entities;

using UnityEngine;

namespace Game.Views
{
    public interface IView : ICastObject, IComponentData
    {
        Transform Transform { get; }
        IEnumerable<T> GetComponents<T>()
            where T : IViewComponent;

        bool HasInitialize { get; }
        void Initialization(IGameEntity entity);
    }
}
