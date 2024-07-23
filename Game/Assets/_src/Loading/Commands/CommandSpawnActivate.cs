using System.Threading.Tasks;

using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using Game.Core.Spawns;

namespace Game.Core.Loading
{
    public class CommandSpawnActivate : ICommand
    {
        public Task Execute()
        {
            return UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();
                Spawner.RemoveWaitTag();
            }).AsTask();
        }
    }
}