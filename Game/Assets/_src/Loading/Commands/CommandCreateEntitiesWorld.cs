using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Common.Core;
using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using Reflex.Attributes;

using Unity.Entities;

using Application = UnityEngine.Application;

namespace Game.Core.Loading
{
    public class CommandCreateEntitiesWorld : ICommandProgress
    {
        [Inject] private IContainer m_Container;
        public float GetProgress()
        {
            return 1;
        }

        public Task Execute()
        {
            return UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();
                //var container = SceneManager.GetActiveScene().GetSceneContainer();

                World.SystemCreated += (world, componentSystemBase) =>
                {
                    m_Container.Inject(componentSystemBase);
                };
                
                World.UnmanagedSystemCreated += async (world, ptr, type) =>
                {
                    var obj = Marshal.PtrToStructure(ptr, type);
                    await UniTask.SwitchToMainThread();
                    m_Container.Inject(obj);
                };
                
                var world = DefaultWorldInitialization.Initialize(Application.productName, false);
            }).AsTask();
        }
    }
}
