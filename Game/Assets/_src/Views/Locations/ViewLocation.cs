using System.Collections.Generic;

using Common.Core;

using Game.Model.Locations;

using UnityEditor;

using UnityEngine;

namespace Game.Views
{
    public class ViewLocation : MonoBehaviour, IView, ISerializationCallbackReceiver
    {
        [SerializeField] private Uuid uuid;
        
#if UNITY_EDITOR
        [SerializeField]
        private PlayerSpawnPointConfig configPoint;
#endif
        [SerializeField]
        private ObjectID config;


        public ObjectID Config => config; 
        public Uuid UID => uuid;
        public Transform Transform => transform;

        IEnumerable<T> IView.GetComponents<T>()
        {
            return GetComponents<T>();
        }
        
        #region UNITY_EDITOR
#if UNITY_EDITOR
        private void OnValidate()
        {
            uuid = uuid.GetUid("locus", GetInstanceID(), out var change);
            if (change) EditorUtility.SetDirty(this);
        }

        private void OnDestroy()
        {
            if(!Application.isPlaying) uuid.Remove();
        }
#endif
        #endregion
        #region ISerializationCallbackReceiver
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (!configPoint) return;
            config = configPoint.ID;
#endif
        }

        public void OnAfterDeserialize() {}
        #endregion
    }
}
