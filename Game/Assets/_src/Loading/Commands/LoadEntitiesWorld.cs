using System.Runtime.InteropServices;
using System.Threading.Tasks;
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

                World.SystemCreated += async (world, componentSystemBase) =>
                {
                    AttributeInjector.Inject(componentSystemBase, container);
                };
                
                World.UnmanagedSystemCreated += async (world, ptr, type) =>
                {
                    var obj = Marshal.PtrToStructure(ptr, type);
                    AttributeInjector.Inject(obj, container);
                };
                
                var world = DefaultWorldInitialization.Initialize("Default World", false);
            }).AsTask();
        }
    }
}
