using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Defs
{
    using Core;
    
    public abstract class ScriptableIdentifiable : ScriptableObject, IIdentifiable<ObjectID>, ISerializationCallbackReceiver
    {
        private ObjectID m_ID;
        public ObjectID ID => m_ID;
        
        public virtual void OnBeforeSerialize()
        {
            if (!this) return;
            m_ID = ObjectID.Create(name);
        }

        public virtual void OnAfterDeserialize()
        {
        }

        private void OnEnable()
        {
            m_ID = ObjectID.Create(name);
        }

        private void OnValidate()
        {
            m_ID = ObjectID.Create(name);
        }
    }
}