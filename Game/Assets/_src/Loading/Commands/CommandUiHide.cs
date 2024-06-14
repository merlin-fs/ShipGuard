using System;
using System.Threading.Tasks;

using Common.Core.Loading;
using Common.UI;

using Game.UI;

using Reflex.Attributes;

namespace Game.Core.Loading
{
    public class CommandUiHide<T> : ICommand
        where T : IWidget
    {
        [Inject] private IUIManager m_UIManager;
        public Task Execute()
        {
            return Task.Run(() => m_UIManager.Hide<T>());
        }
    }
}