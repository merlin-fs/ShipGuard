using Common.Core;
using Common.Core.Loading;

using Game.Core.Loading;

using Reflex.Attributes;

using UnityEngine.UIElements;

namespace Game.UI
{
    public class MainMenu : UIWidget
    {
        [Inject] private IContainer m_Container;
        [Inject] private ILoadingManager m_LoadingManager;
        
        protected override void Bind()
        {
            RootVisualElement.Q<Button>("new")
                .RegisterCallback<ClickEvent>(e =>
                {
                    m_LoadingManager.Start(m_Container, new []{new LoadingManager.CommandItem(new LoadUI())});
                });
        }
    }
}
