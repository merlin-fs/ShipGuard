using System;

using Common.Core;

using Game.Core.Repositories;
using Game.Model;
using Game.Model.Locations;

using Unity.Entities;
using Unity.Transforms;

namespace Game.Core.Spawns
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(GameSpawnSystemGroup))]
    [UpdateBefore(typeof(Spawn.System))]
    public partial struct LocationSpawnSystem : ISystem, ISystemStartStop
    {
        private static GameEntityRepository m_GameEntityRepository;
        private static LocationViewRepository m_LocationViewRepository;

        private static Action<EntityCommandBuffer> m_OnInitialization;
        
        public void Inject(IContainer container)
        {
            m_GameEntityRepository = container.Resolve<GameEntityRepository>();
            m_LocationViewRepository = container.Resolve<LocationViewRepository>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var system = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = system.CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (gameEntity, entity) in SystemAPI.Query<GameEntity>()
                         .WithAll<LocationTag, Spawn.PostTag, Spawn.ViewAttachTag>()
                         .WithEntityAccess())
            {
                var view = m_LocationViewRepository.FindByID(gameEntity.ID);
                view.Initialization(gameEntity);
                m_GameEntityRepository.Insert(gameEntity.ID, gameEntity);
                
                ecb.RemoveComponent<Spawn.ViewAttachTag>(entity);
            }

            foreach (var (transform, link, entity) in SystemAPI.Query<RefRW<LocalTransform>, LocationLink>()
                         .WithAll<Spawn.PostTag>()
                         .WithEntityAccess())
            {
                var view = m_LocationViewRepository.FindByID(link.LocationId);
                transform.ValueRW.Position = view.Transform.position;
                transform.ValueRW.Rotation = view.Transform.rotation;
            }
        }

        public void AddInitialization(Action<EntityCommandBuffer> onInitialization)
        {
            m_OnInitialization = onInitialization;
        }
        
        public void OnStartRunning(ref SystemState state)
        {
            if (m_OnInitialization == null) return;
            
            var system = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = system.CreateCommandBuffer(state.WorldUnmanaged);
            m_OnInitialization.Invoke(ecb);
        }

        public void OnStopRunning(ref SystemState state)
        {
            
        }
    }
}
