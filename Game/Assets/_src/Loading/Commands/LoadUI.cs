using System.Threading.Tasks;
using Common.Core.Loading;
using Common.UI;

using Cysharp.Threading.Tasks;

using Reflex.Extensions;

using UnityEngine.SceneManagement;

namespace Game.Core.Loading
{
    public class LoadUI : ILoadingCommand
    {
        public float GetProgress()
        {
            return 1;
        }

        public Task Execute(ILoadingManager manager)
        {
            return UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();
                var container = SceneManager.GetActiveScene().GetSceneContainer();
                //var uiManager = container.Resolve<IUIManager>();
                //uiManager.Show<GameUI>(true);
            }).AsTask();
        }
    }
}