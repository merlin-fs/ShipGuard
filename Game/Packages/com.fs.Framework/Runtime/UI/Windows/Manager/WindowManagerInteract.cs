using System;
using System.Collections;
using System.Collections.Generic;

namespace Common.UI.Windows
{
	public partial class WindowManager
	{
		StackWindow m_Stack = new StackWindow();

		void TryExecInteract()
		{
			if (m_Stack.IsActive)
				return;

			if (m_WindowsRegister.Count > 0)
			{
				StartCoroutine(HideLastWindowProc());
				StartCoroutine(OpenWindowProc());
				return;
			}

			if (m_WindowsUnregister.Count > 0)
			{
				StartCoroutine(CloseWindowProc());
				StartCoroutine(ShowLastWindowProc());
				return;
			}
		}

		IEnumerator OpenWindowProc()
		{
			IWindowKernel window = m_WindowsRegister[0];
			m_Stack.Add(window);
			try
			{
				m_WindowsRegister.RemoveAt(0);
				m_Windows.Add(window);
				RecalcLayers();

				window.InitWindow();
                
                //TODO: решить проблему с тасками
                yield return null;
                /*
                yield return window.Animate(IWindow.AnimationMode.Open, m_InteractiveAnimateTime).Play()
					.AsUniTask()
					.ToCoroutine();
                */
			}
            finally
            {
				RecalcLayers();
				m_Stack.Remove(window);
			}
		}

		IEnumerator HideLastWindowProc()
		{
			if (m_Windows.Count == 0)
				yield break;

			IWindowKernel window = m_Windows[m_Windows.Count - 1];
			RecalcLayers();

			if (!window.IsWindowHidden)
			{
				m_Stack.Add(window);
				try
				{
                    //TODO: решить проблему с тасками
                    yield return null;
                    /*
                    yield return window.Animate(IWindow.AnimationMode.Hide, m_InteractiveAnimateTime).Play()
						.AsUniTask()
						.ToCoroutine();
                    */
				}
				finally
				{
					RecalcLayers();
					m_Stack.Remove(window);
				}
			}
		}

		IEnumerator ShowLastWindowProc()
		{
			if (m_Windows.Count == 0)
				yield break;
			
			IWindowKernel window = m_Windows[m_Windows.Count - 1];
			RecalcLayers();

			if (window.IsWindowHidden)
			{
				m_Stack.Add(window);
				try
				{
                    //TODO: решить проблему с тасками
                    yield return null;
                    /*
                    yield return window.Animate(IWindow.AnimationMode.Show, m_InteractiveAnimateTime).Play()
						.AsUniTask()
						.ToCoroutine();
                    */
				}
				finally
				{
					RecalcLayers();
					m_Stack.Remove(window);
				}
			}
		}

		IEnumerator CloseWindowProc()
		{
			IWindowKernel window = m_WindowsUnregister[0];

			m_WindowsUnregister.RemoveAt(0);
			m_Windows.Remove(window);
			RecalcLayers();

			if (!window.IsWindowHidden)
			{
				m_Stack.Add(window);
				try
                {
                    //TODO: решить проблему с тасками
                    yield return null;
                    /*
                    yield return window.Animate(IWindow.AnimationMode.Close, m_InteractiveAnimateTime).Play()
						.AsUniTask()
						.ToCoroutine();
                    */
				}
                finally
                {
					RecalcLayers();
					m_Stack.Remove(window);
				}
			}
			window.Dispose();
		}

		// ---------------------------------------------------------------------------------

		class StackWindow
		{
			List<IWindowKernel> m_Windows = new List<IWindowKernel>();

			public bool IsActive => m_Windows.Count > 0;

			public void Add(IWindowKernel window)
			{
				m_Windows.Add(window); 
			}

			public void Remove(IWindowKernel window)
			{
				m_Windows.Remove(window); 
			}
		}
	}
}