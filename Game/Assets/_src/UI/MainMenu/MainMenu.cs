using System.Collections.Generic;

using Common.Core.Loading;
using Common.UI;

using Reflex.Attributes;

using UnityEngine.UIElements;

namespace Game.UI
{
    public class MainMenu : UIWidget
    {
        [Inject] private ILoadingManager m_LoadingManager;
        
        protected override void Bind()
        {
            Document.rootVisualElement.Q<Button>("new")
                .RegisterCallback<ClickEvent>(e =>
                {
                    m_LoadingManager.Start();
                });
        }

        public override IEnumerable<VisualElement> GetElements()
        {
            throw new System.NotImplementedException();
        }
    }
}
