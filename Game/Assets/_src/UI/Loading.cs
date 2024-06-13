using Common.Core;
using Common.UI;

using Reflex.Attributes;

using UnityEngine.UIElements;

namespace Game.UI
{
    public class Loading : IWidget<IProgress>
    {
        [Inject] private WidgetConfig m_WidgetConfig;

        public void Bind(VisualElement root, IProgress progress)
        {
            var progressBar = root.Q<ProgressBar>("progress");
            progressBar.value = progress.Value;
            progressBar.schedule.Execute(() => progressBar.value = progress.Value).Every(10);
        }

        public VisualTreeAsset GetTemplate()
        {
            m_WidgetConfig.TryGetTemplate<Loading>(out var template);
            return template;
        }
    }
}
