using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;

namespace Common.UI.Windows
{
	public class SubstrateButton : MonoBehaviour, IPointerClickHandler
	{
		[SerializeField]
		UnityEvent m_OnClick = default;

        [SerializeField]
		bool m_IsUseWindowExitHandler = true;

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{ 
			m_OnClick.Invoke(); 
		}

		void Awake()
		{
			if (m_IsUseWindowExitHandler)
				gameObject.AddComponent<WindowExitHandler>();
		}
	}
}