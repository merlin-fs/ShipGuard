using System;

using Common.Core;

using Game.Core.HybridTransforms;
using Game.Core.Repositories;
using Game.Model;
using Game.Model.Locations;

using Unity.Entities;
using Unity.Transforms;

namespace Game.Core.Spawns
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(GameSpawnSystemGroup))]
    [UpdateAfter(typeof(Spawn.System))]
    public partial struct LocationSpawnSystem : ISystem, ISystemStartStop
    {
        private static LocationViewRepository m_LocationViewRepository;

        private static Action<EntityCommandBuffer> m_OnInitialization;
        private static Action<EntityCommandBuffer> m_OnFinalization;

        public void Inject(IContainer container)
        {
            m_LocationViewRepository = container.Resolve<LocationViewRepository>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var system = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = system.CreateCommandBuffer(state.WorldUnmanaged);
            
            foreach (var (transform, gameEntity, uniqueEntity, entity) in SystemAPI.Query<RefRW<LocalTransform>, GameEntity, UniqueEntity>()
                         .WithAll<LocationTag, Spawn.PostTag>()
                         .WithNone<Spawn.WaitSpawnTag>()
                         .WithEntityAccess())
            {
                var view = m_LocationViewRepository.FindByID(uniqueEntity.ID);
                view.Initialization(gameEntity);
                
                transform.ValueRW.Position = view.Transform.position;
                transform.ValueRW.Rotation = view.Transform.rotation;

                ecb.AddComponent(entity, new HybridTransform.ReferenceView{ Value = view });
            }
        }

        public void SetInitializationEvent(Action<EntityCommandBuffer> onInitialization)
        {
            m_OnInitialization = onInitialization;
        }

        public void SetFinalizationEvent(Action<EntityCommandBuffer> onFinalization)
        {
            m_OnFinalization = onFinalization;
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
            if (m_OnFinalization == null) return;
            
            var system = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = system.CreateCommandBuffer(World.DefaultGameObjectInjectionWorld.Unmanaged);
            m_OnFinalization.Invoke(ecb);
        }
    }
}
