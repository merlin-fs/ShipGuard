using System;
using System.Threading.Tasks;

using Common.Core.Loading;
using Common.UI;

using Game.UI;

using Reflex.Attributes;

namespace Game.Core.Loading
{
    public class CommandUiShow<T> : ICommand
        where T : IWidget
    {
        [Inject] private IUIManager m_UIManager;
        private UILayer m_UILayer;

        public CommandUiShow(UILayer layer)
        {
            m_UILayer = layer;
        }
        
        public Task Execute()
        {
            return Task.Run(() => m_UIManager.Show<T>().WithLayer(m_UILayer));
        }
    }
}