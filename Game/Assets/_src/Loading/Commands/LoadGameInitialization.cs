using System.Threading.Tasks;
using Common.Core.Loading;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Reflex.Core;

namespace Game.Core.Loading
{
    public class LoadGameInitialization : ILoadingCommand
    {
        [Inject] private IInitialization m_Initialization;
        
        public float GetProgress()
        {
            return 1;
        }

        public Task Execute(ILoadingManager manager)
        {
            return UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();
                m_Initialization.Initialization();
            }).AsTask();
        }
    }
}