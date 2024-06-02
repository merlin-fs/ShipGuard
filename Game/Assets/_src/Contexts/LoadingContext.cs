using Common.Core.Loading;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    public class LoadingContext: MonoBehaviour
    {
        [SerializeField] private UIDocument rootUI;
        [Inject] private ILoadingManager m_LoadingManager;

        private void Awake()
        {
            var progress = rootUI.rootVisualElement.Q<ProgressBar>("progress");
            progress.schedule.Execute(() => progress.value = m_LoadingManager.Progress.Value).Every(10);
        }
    }
}