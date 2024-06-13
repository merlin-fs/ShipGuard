using System;
using System.Linq;

using Common.Core;
using Common.Core.Loading;
using Common.UI;

using Game.UI;

using Reflex.Attributes;

using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    public class InitializationTestUI : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset template;

        [Inject] private IUIManager<UILayer> m_UIManager;
        [Inject] private IInitialization m_Initialization;

        void Start()
        {
            m_Initialization.Initialization(null);

            //ILoadingManager loading = new LoadingManager(null, new LoadingManager.CommandItem[]{});

            /*
            m_UIManager.Show<Loading>()
                .WithLayer(UILayer.Loading)
                .WithData(() => loading.Progress);
            */

            m_UIManager.Show<Loading>()
                .WithLayer(UILayer.Main);

            m_UIManager.Show<Loading>()
                .WithLayer(UILayer.Windows);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
