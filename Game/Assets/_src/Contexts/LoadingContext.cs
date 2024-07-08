using Common.Core.Loading;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Core.Contexts
{
    public class LoadingContext: MonoBehaviour
    {
        [SerializeField] private UIDocument loadingUI;
        [Inject] private ILoadingManager m_LoadingManager;
        
        private void Awake()
        {
            var widget = new UI.Loading(m_LoadingManager);
            //TODO: !!!
            //widget.Bind(loadingUI.rootVisualElement);
        }
    }
}