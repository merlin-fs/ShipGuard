using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.UI.Windows
{
	using Singletons;

	public interface IWindowManager: ISingleton
	{
		IAnimation WindowAnimatorDefault { get; }
		
		void RegisterWindow(IWindowKernel window);
		
		void UnregisterWindow(IWindowKernel window);
		
		bool IsBusy();

        void SetDarkVisible(bool value);
		GameObject Instantiate(GameObject obj);
		
		Transform Transform { get; }
	}

	public partial class WindowManager : MonoBehaviour, IWindowManager
	{
		//[SerializeField]
		//private float m_InteractiveAnimateTime = 0.2f;

		[SerializeField]
		private Transform m_Dark = default;

		[SerializeField]
		private IAnimation m_WindowAnimatorDefault = default;

		List<IWindowKernel> m_Windows = new List<IWindowKernel>();
		List<IWindowKernel> m_WindowsRegister = new List<IWindowKernel>();
		List<IWindowKernel> m_WindowsUnregister = new List<IWindowKernel>();

		// ---------------------------- IWindowManager ----------------------------

		IAnimation IWindowManager.WindowAnimatorDefault => m_WindowAnimatorDefault;

		void IWindowManager.RegisterWindow(IWindowKernel window)
		{
			window.Transform.SetParent(transform, false);
			m_WindowsRegister.Add(window);
			//InputLockProc();
		}

		void IWindowManager.UnregisterWindow(IWindowKernel window)
		{
			m_WindowsUnregister.Add(window);
			//InputLockProc();
		}


        void IWindowManager.SetDarkVisible(bool value)
        {
            if (m_Dark)
                m_Dark.gameObject.SetActive(value);
        }

        public bool IsBusy()
		{ 
			return m_Windows.Count > 0 
				|| m_WindowsRegister.Count > 0 
				|| m_WindowsUnregister.Count > 0 
				|| m_Stack.IsActive; 
		}

		GameObject IWindowManager.Instantiate(GameObject obj)
		{
			GameObject go = Instantiate(obj, transform) as GameObject;
			var windown = go.GetComponent<IWindowKernel>();

			if (windown.WindowMode == IWindow.Mode.Popup)
				go.AddComponent<WindowExitHandler>();
			return go;
		}

		Transform IWindowManager.Transform => transform;

		// ---------------------------------------------------------------------------

		private void Update()
		{
			TryExecInteract();
			//InputLockProc();
		}

		void RecalcLayers()
		{
			if (m_Windows.Count > 0)
			{
				for (int i = 0; i < m_Windows.Count; i++)
				{
					if (i == m_Windows.Count - 1 && m_Dark != null)
					{
                        if (m_Dark)
                            m_Dark.SetSiblingIndex(i);
						m_Windows[i].SetSiblingIndex(i + 1);
					}
					else
						m_Windows[i].SetSiblingIndex(i);
				}
			}
			else
			{
                if (m_Dark)
                    m_Dark.gameObject.SetActive(false);
			}
		}
	}
}