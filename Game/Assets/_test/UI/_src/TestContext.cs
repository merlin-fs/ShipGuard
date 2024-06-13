using Common.Core;
using Common.Core.Loading;
using Common.UI;

using Game.UI;

using Reflex.Attributes;
using Reflex.Core;

using UnityEngine;

namespace Game
{
    public class TestContext : MonoBehaviour, IInstaller
    {
        [SerializeField] private GameObject uiManagerHost;
        //[Inject] private ILoadingManager loadingManager;
        [Inject] private WidgetConfig widgetConfig;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton<IUIManager<UILayer>>(c => 
                c.Construct<UIManager<UILayer>>(uiManagerHost, UILayer.Main, widgetConfig.GetLayers));
        }
    }
}
