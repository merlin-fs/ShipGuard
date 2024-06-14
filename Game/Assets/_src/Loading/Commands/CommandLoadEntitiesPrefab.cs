using System.Threading.Tasks;
using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using Game.Core.Defs;
using Game.Core.Repositories;

using Reflex.Attributes;

using Unity.Entities;

namespace Game.Core.Loading
{
    public class CommandLoadEntitiesPrefab : ICommandProgress
    {
        [Inject] private ObjectRepository m_ObjectRepository;

        public float GetProgress()
        {
            return 1;
        }

        public Task Execute()
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
