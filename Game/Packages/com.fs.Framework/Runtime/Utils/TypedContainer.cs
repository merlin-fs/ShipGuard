using System;
using UnityEngine;

namespace System
{
	[Serializable]
	public abstract class ATypedContainer
	{
		[SerializeField]
		protected UnityEngine.Object m_Obj = default;

		public void Validate()
		{
			OnValidate();
		}

		public void SetObject(UnityEngine.Object obj)
        {
			m_Obj = obj;
		}

		protected abstract void OnValidate();
	}

	[Serializable]
	public class TypedContainer<T> : ATypedContainer where T : class
	{
		public T Value => m_Obj as T;

		protected override void OnValidate()
		{
			T tmp = m_Obj as T;
			if (tmp == null && m_Obj is MonoBehaviour beh)
            {
                m_Obj = beh.GetComponent<T>() as UnityEngine.Object;
            }
			if (Value == null)
				m_Obj = null;
		}
	}
}
