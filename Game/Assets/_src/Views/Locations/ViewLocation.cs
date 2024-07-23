using System.Collections.Generic;

using Common.Core;

using Game.Model;
using Game.Model.Locations;

using UnityEngine;

namespace Game.Views
{
    public class ViewLocation : MonoBehaviour, IView
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
    {
#if UNITY_EDITOR
        [SerializeField]
        private PointConfig configPoint;
#endif
        [SerializeField]
        private ObjectID configId;
        public ObjectID ConfigId => configId; 
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
        #region ISerializationCallbackReceiver
#if UNITY_EDITOR
        public void OnBeforeSerialize()
        {
            if (!configPoint) return;
            configId = configPoint.ID;
        }

        public void OnAfterDeserialize() {}
#endif
        #endregion
    }
}
