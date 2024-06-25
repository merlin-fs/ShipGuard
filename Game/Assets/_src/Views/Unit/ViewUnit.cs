using System.Collections.Generic;

using Game.Model;

using UnityEngine;

namespace Game.Views
{
    public class ViewUnit : MonoBehaviour, IView
    {
        protected IGameEntity GameEntity { get; private set; }
        public Transform Transform => transform;

        IEnumerable<T> IView.GetComponents<T>()
        {
            return GetComponents<T>();
        }

        public bool HasInitialize => GameEntity != null;

        public void Initialization(IGameEntity entity)
        {
            GameEntity = entity;
        }

        public T Cast<T>() => GetComponent<T>();
    }
}
