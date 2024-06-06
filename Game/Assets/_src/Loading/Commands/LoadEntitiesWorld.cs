using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Common.Core;
using Common.Core.Loading;
using Cysharp.Threading.Tasks;

using Reflex.Attributes;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;

using Unity.Collections;
using Unity.Entities;

using UnityEngine.SceneManagement;

namespace Game
{
    public class LoadEntitiesWorld : ILoadingCommand
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

                World.SystemCreated += (world, componentSystemBase) =>
                {
                    container.Inject(componentSystemBase);
                };
                
                World.UnmanagedSystemCreated += (world, ptr, type) =>
                {
                    var obj = Marshal.PtrToStructure(ptr, type);
                    container.Inject(obj);
                };
                
                var world = DefaultWorldInitialization.Initialize("Default World", false);
            }).AsTask();
        }
    }
}
