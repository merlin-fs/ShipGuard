using UnityEngine;

namespace Game.Core
{
    public abstract class MonoBehaviourEditorProperties : MonoBehaviour
#if !UNITY_EDITOR
{}
#else
        , ISerializationCallbackReceiver
    {
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            OnEditorSerialize();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            OnEditorDeserialize();
        }
        protected virtual void OnEditorSerialize() { }

        protected virtual void OnEditorDeserialize() { }
    }
#endif
}
