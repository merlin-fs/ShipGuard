using System.Threading.Tasks;
using Common.Core.Loading;
using Common.Defs;

using Cysharp.Threading.Tasks;

using Game.Core.Prefabs;
using Game.Core.Repositories;

using Reflex.Attributes;

using Unity.Collections;
using Unity.Entities;

namespace Game
{
    public class LoadEntitiesPrefab : ILoadingCommand
    {
        [Inject] private ObjectRepository m_ObjectRepository;

        public float GetProgress()
        {
            return 1;
        }

        public Task Execute(ILoadingManager manager)
        {
            return UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();                
                var worldUnmanaged = World.DefaultGameObjectInjectionWorld.EntityManager.WorldUnmanaged;
                var ecb = worldUnmanaged.EntityManager.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                    .CreateCommandBuffer();
                    
                //var context = new CommandBufferContext(ecb);
                var context = new EntityManagerContext(worldUnmanaged.EntityManager);

                foreach (var config in m_ObjectRepository.Find())
                {
                    var entity = worldUnmanaged.EntityManager.CreateEntity();
                    context.AddComponentData(entity, new Prefab{});
                    config.Configure(entity, context);
                }
            }).AsTask();
        }
    }
}
