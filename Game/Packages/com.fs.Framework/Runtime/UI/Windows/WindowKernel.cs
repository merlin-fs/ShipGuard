using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.UI.Windows
{
	using Core;

	public partial class Window
    {
        //TODO: доробити
        private IWindowManager m_WindowManager;

        IAnimation m_Anim;

		private struct InitData
        {
			public Action Init;
			public IViewModel Model;
		}

		private readonly List<InitData> m_DeferredInit = new List<InitData>();

		private bool m_IsCallWindowInit;
		private bool m_IsCallCloseWindow;
		private bool m_IsDestroy;
		private bool m_IsWindowHidden;

		public event Action<IWindow.AnimationMode> OnAnimStart;
        
		public event Action<IWindow.AnimationMode> OnAnimComplate;
		public event Action OnWindowClose;

		IWindow.Mode IWindowKernel.WindowMode => m_WindowMode;
		Transform IWindowKernel.Transform => transform;
		bool IWindowKernel.IsWindowHidden => m_IsWindowHidden;

		IAnimation IWindowKernel.Animate(IWindow.AnimationMode mode, float time)
		{
			m_IsWindowHidden = mode == IWindow.AnimationMode.Close || mode == IWindow.AnimationMode.Hide;
			OnAnimStart?.Invoke(mode);
			IAdditionalBehaviour[] behaviours = GetComponents<IAdditionalBehaviour>();
            if (behaviours.Length == 0)
            {
                var anim = gameObject.AddComponent<DefaultAnim>();
                behaviours = new IAdditionalBehaviour[] { anim };
            }

            foreach (IAdditionalBehaviour iter in behaviours)
				iter.Play(mode, time);

			if (m_Anim == null)
				return new WindowAnimateHandler();
			return null;
		}

        internal void DoAnimComplate(IWindow.AnimationMode mode)
        {
            OnAnimComplate?.Invoke(mode);
        }

		void IWindowKernel.InitWindow()
		{
			gameObject.SetActive(true);
			m_IsCallWindowInit = true;
			var view = GetComponent<IView>();
			foreach (var method in m_DeferredInit)
            {
				view?.Initialize(method.Model);
				method.Init.Invoke();
			}
		}

		public virtual void Dispose()
		{
			if (m_IsDestroy)
				return;
			m_IsDestroy = true;
			OnWindowClose?.Invoke();
			Destroy(gameObject);
		}

		void IWindowKernel.SetSiblingIndex(int index)
		{ 
			transform.SetSiblingIndex(index); 
		}

		void IWindow.CloseWindow(Action onWindowClose)
		{
			if (m_IsDestroy)
			{
				onWindowClose?.Invoke();
				return;
			}
			OnWindowClose += onWindowClose;
			DoCloseWindow();
		}
        
        protected override void Awake()
		{
			m_Anim = transform.GetComponent<IAnimation>();
			if (m_Anim == null)
				m_Anim = m_WindowManager.WindowAnimatorDefault;
		}

		protected override void Start()
        {
			gameObject.SetActive(false);
            m_WindowManager.RegisterWindow(this);
		}

		private void DoCloseWindow()
		{
			if (m_IsCallCloseWindow)
				return;
			m_IsCallCloseWindow = true;
            m_WindowManager.UnregisterWindow(this);
		}
	}
}