using System;
using UnityEngine;

namespace Common.UI.Windows
{
	using Core;

	public class WindowPrefabPathAttribute: Attribute
    {
		public string Path { get; }

		public WindowPrefabPathAttribute(string path)
        {
			Path = path;
		}
	}

	[Serializable]
	public abstract class Window<T, M> : MonoBehaviour, IView<M>
		where T : Window<T, M>
		where M : IViewModel
    {
        //TODO: доробити
        private IWindowManager m_WindowManager;
        IViewModel IView.DataSource => DataSource;
		GameObject IView.GameObject => gameObject;
        public M DataSource { get; private set; }

        void IView.Initialize(IViewModel dataSource) => DataSource = (M)dataSource;

		public void Initialize(M dataSource) => DataSource = dataSource;

		public static T Create(M model)
        {
			
			return InstCreate(model, (me) => me.Init());
		}

		public void OnClose(Action onClose)
        {
			Wnd.OnWindowClose += OnClose;

			void OnClose()
			{
				onClose.Invoke();
				Wnd.OnWindowClose -= OnClose;
			}
		}

		private static T InstCreate(M model, Action<T> init)
		{
			Type type = typeof(T);

			var attr = (WindowPrefabPathAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(WindowPrefabPathAttribute));
			string path = attr?.Path ?? type.Name;
			GameObject prefab = Resources.Load(path) as GameObject;

			GameObject go = WindowManager.Instantiate(prefab);
			T me = (T)go.GetComponent(type);
			Window window = go.GetComponent<Window>();
			window.OnWindowClose += me.OnWindowClose;

			window.PushInit(model,
				() =>
				{
					init(me);
				});
			return me;
		}

		protected Window Wnd => GetComponent<Window>();

		protected abstract void Init();

		protected virtual void OnWindowClose()
		{
			DataSource = default;
		}

		public void CloseWindow()
		{
			Wnd.CloseWindow();
		}

		public void Dispose()
        {
			DataSource = default;
			Wnd.CloseWindow();
		}
	}
}
