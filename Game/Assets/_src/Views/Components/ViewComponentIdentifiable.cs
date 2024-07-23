using Common.Core;

using UnityEngine;

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
            if (Application.isPlaying) return;
            uuid = uuid.GetUid("locus", GetInstanceID(), out var change);
            if (change) UnityEditor.EditorUtility.SetDirty(this);
        }

        private void OnDestroy()
        {
            if(!Application.isPlaying) uuid.Remove();
        }
#endif
        #endregion

        public void Initialization(IView view)
        {
        }
    }
}
