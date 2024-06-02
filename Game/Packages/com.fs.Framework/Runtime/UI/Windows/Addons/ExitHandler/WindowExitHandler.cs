using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.UI.Windows
{
	public interface IWindowExitHandler
	{
		void SendClose();
	}

    //TODO: доробити
	public class WindowExitHandler : MonoBehaviour, IWindowExitHandler
    {
        private IWindowExitHandlerPool m_Pool;
        
		private void Awake()
		{
            m_Pool.Push(this);
		}

		private void OnDestroy()
		{
            m_Pool.Pop(this);
		}

		void IWindowExitHandler.SendClose()
		{
			if (GetComponent<Window>() != null)
				GetComponent<Window>().CloseWindow();
			else
			{
				PointerEventData pointer = new PointerEventData(EventSystem.current);
				ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerClickHandler);
			}
		}
	}
}