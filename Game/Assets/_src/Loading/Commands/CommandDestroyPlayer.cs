using System.Threading.Tasks;

using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using Game.Model.Units;

using Reflex.Attributes;

namespace Game.Core.Loading
{
    public class CommandDestroyPlayer : ICommand
    {
        [Inject] private UnitManager m_UnitManager;

        public Task Execute()
        {
            return UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();
                m_UnitManager.DestroyPlayer();
            }).AsTask();
        }
    }
}