using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.UI.Windows
{
	using Core;

	public interface IWindowKernel: IDisposable
	{
		IWindow.Mode WindowMode { get; }
		Transform Transform { get; }
		bool IsWindowHidden { get; }
		IAnimation Animate(IWindow.AnimationMode mode, float time);
		void InitWindow();
		void SetSiblingIndex(int index);
	}

	public interface IWindow
	{
		public enum Mode
		{
			Window,
			Popup,
			Widget
		}

		public enum AnimationMode
		{
			Open,
			Close,
			Hide,
			Show,
		}

		void CloseWindow(Action onWindowClose = null);
		event Action<AnimationMode> OnAnimStart;
		event Action<AnimationMode> OnAnimComplate;
		event Action OnWindowClose;
	}

	public partial class Window : UIBehaviour, IWindowKernel, IWindow
	{
		[SerializeField]
		private IWindow.Mode m_WindowMode = IWindow.Mode.Window;

		public void PushInit(IViewModel model, Action method)
		{
			m_DeferredInit.Add(new InitData() { Init = method, Model = model });
			if (m_IsCallWindowInit)
				method.Invoke();
		}

		public void CloseWindow()
		{
			DoCloseWindow();
		}
	}
}