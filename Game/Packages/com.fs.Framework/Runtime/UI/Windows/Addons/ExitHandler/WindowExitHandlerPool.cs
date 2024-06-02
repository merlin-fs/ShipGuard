using System.Collections.Generic;
using UnityEngine;

namespace Common.UI.Windows
{
	public interface IWindowExitHandlerPool
	{
		void Push(IWindowExitHandler window);
		void Pop(IWindowExitHandler window);
	}

	public class WindowExitHandlerPool : IWindowExitHandlerPool
	{
        //TODO: доробити
        private IWindowManager m_WindowManager;
		List<IWindowExitHandler> m_WidowStack = new List<IWindowExitHandler>();

		protected virtual void OnTryTerminate() { }

		public void Update()
		{
			if (!Input.GetKeyDown(KeyCode.Escape))
				return;

			if (m_WidowStack.Count > 0)
				m_WidowStack[m_WidowStack.Count - 1].SendClose();
			else
			{
				if (m_WindowManager.IsBusy())
					return;

				OnTryTerminate();
			}
		}

		void IWindowExitHandlerPool.Push(IWindowExitHandler window)
		{ 
			m_WidowStack.Add(window); 
		}

		void IWindowExitHandlerPool.Pop(IWindowExitHandler window)
		{ 
			m_WidowStack.Remove(window); 
		}
	}
}