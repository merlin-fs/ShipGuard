using System;
using UnityEngine;


namespace System.Collections.Generic
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> m_ValueKeys = new List<TKey>();

        [SerializeField]
        private List<TValue> m_ValueDatas = new List<TValue>();

        public SerializedDictionary() : base()
        {
        }

        public SerializedDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < m_ValueKeys.Count && i < m_ValueDatas.Count; i++)
            {
                this[m_ValueKeys[i]] = m_ValueDatas[i];
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_ValueKeys.Clear();
            m_ValueDatas.Clear();

            foreach (var item in this)
            {
                m_ValueKeys.Add(item.Key);
                m_ValueDatas.Add(item.Value);
            }
        }
    }
}
