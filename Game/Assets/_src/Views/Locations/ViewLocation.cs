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
        private PlayerSpawnPointConfig configPoint;
#endif
        [SerializeField]
        private ObjectID config;
        public ObjectID Config => config; 
        public Transform Transform => transform;
        
        protected IGameEntity GameEntity { get; private set; } 

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
            config = configPoint.ID;
        }

        public void OnAfterDeserialize() {}
#endif
        #endregion
    }
}
