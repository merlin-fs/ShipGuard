using Common.Core;

using Game.Core.Repositories;
using Game.Model;
using Game.Model.Locations;

using Unity.Entities;

namespace Game.Core.Spawns
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(GameSpawnSystemGroup))]
    public partial struct LocationSpawnSystem : ISystem
    {
        private static LocationRepository m_LocationRepository;
        private static LocationViewRepository m_LocationViewRepository;

        public void Inject(IContainer container)
        {
            m_LocationRepository = container.Resolve<LocationRepository>();
            m_LocationViewRepository = container.Resolve<LocationViewRepository>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var system = SystemAPI.GetSingleton<GameSpawnSystemCommandBufferSystem.Singleton>();
            var ecb = system.CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (gameEntity, entity) in SystemAPI.Query<GameEntity>()
                         .WithAll<LocationTag, Spawn.PostSpawnTag, Spawn.ViewAttachTag>()
                         .WithEntityAccess())
            {
                var view = m_LocationViewRepository.FindByID(gameEntity.ID);
                view.Initialization(gameEntity);
                m_LocationRepository.Insert(gameEntity.ID, gameEntity);
                
                ecb.RemoveComponent<Spawn.ViewAttachTag>(entity);
            }
        }
    }
}
