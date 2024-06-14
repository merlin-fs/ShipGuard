using System.Threading.Tasks;

using Common.Core;
using Common.Core.Loading;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;

namespace Game.Core.Loading
{
    public class CommandStartGameInitialization : ICommand
    {
        [Inject] private IInitialization m_Initialization;

        public Task Execute()
        {
            return UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();
                m_Initialization.Initialization(null);
            }).AsTask();
        }
    }
}