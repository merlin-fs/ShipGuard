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
            //containerBuilder.AddSingleton(widgetConfig);
            containerBuilder.AddSingleton<ILoadingManager>(c => c.Construct<LoadingManager>(c, new ICommandItem[] { })); 
            containerBuilder.AddSingleton<IUIManager>(container =>
            {
                var manager = container.Construct<UIManager>(
                    container,
                    uiManagerHost,
                    UILayer.Main,
                    widgetConfig.GetLayers);
                return manager;
            });
        }
    }
}
