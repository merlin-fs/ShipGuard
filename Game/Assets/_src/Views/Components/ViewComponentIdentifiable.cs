using Common.Core;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Views
{
    public class ViewComponentIdentifiable : MonoBehaviour, IViewComponent, IIdentifiable<Uuid>
    {
        [SerializeField] private Uuid uuid;
        public Uuid ID => uuid;

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
    }
}
