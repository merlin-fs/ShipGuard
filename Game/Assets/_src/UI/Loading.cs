using Common.Core.Loading;
using Common.UI;

using Reflex.Attributes;

using UnityEngine.UIElements;

namespace Game.UI
{
    public class Loading : IWidgetNoData<VisualElement>
    {
        [Inject] private WidgetConfig m_WidgetConfig;
        [Inject] private ILoadingManager m_LoadingManager;

        public Loading(){}

        public Loading(ILoadingManager loadingManager)
        {
            m_LoadingManager = loadingManager;
        }

        public void Bind(Binder<VisualElement> binder)
        {
            var progressBar = binder.Element.Q<ProgressBar>("progress");
            progressBar.value = m_LoadingManager.Progress.Value;
            progressBar.schedule.Execute(() => progressBar.value = m_LoadingManager.Progress.Value).Every(10);
        }

        public VisualTreeAsset GetTemplate()
        {
            m_WidgetConfig.TryGetTemplate<Loading>(out var template);
            return template;
        }
    }
}
