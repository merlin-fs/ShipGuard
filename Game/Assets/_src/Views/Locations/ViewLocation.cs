using System.Collections.Generic;

using Common.Core;

using Game.Model;
using Game.Model.Locations;

using UnityEngine;

namespace Game.Views
{
    public class ViewLocation : MonoBehaviour, IView
    {
        public Transform Transform => transform;
        public IGameEntity GameEntity { get; private set; }
        T ICastObject.Cast<T>() => GetComponent<T>();

        IEnumerable<T> IView.GetComponents<T>()
        {
            return GetComponents<T>();
        }

        public bool HasInitialize => GameEntity != null;

        public void Initialization(IGameEntity entity)
        {
            GameEntity = entity;
        }
    }
}
