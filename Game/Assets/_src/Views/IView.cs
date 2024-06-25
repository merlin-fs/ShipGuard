using System.Collections.Generic;

using Common.Core;

using Game.Model;

using UnityEngine;

namespace Game.Views
{
    public interface IView : ICastObject
    {
        Transform Transform { get; }
        IEnumerable<T> GetComponents<T>()
            where T : IViewComponent;

        bool HasInitialize { get; }
        void Initialization(IGameEntity entity);
    }
}
