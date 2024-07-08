using Common.Core;

using Game.Core.Repositories;
using Game.Core.Spawns;

using Reflex.Attributes;
using Reflex.Core;

using Unity.Entities;

using UnityEngine;

namespace Game.Core.Contexts
{
    public class LocationSceneContext : MonoBehaviour, IInstaller
    {
        [Inject] private LocationManager m_LocationManager;
        
        private SystemHandle m_LocationSpawnSystem;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(c => c.Construct<LocationViewRepository>());
            containerBuilder.AddSingleton(c => c.Construct<GameEntityRepository>());
        }

        public void Initialization(IContainer container)
        {
            m_LocationSpawnSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<LocationSpawnSystem>();
            var system =
                World.DefaultGameObjectInjectionWorld.Unmanaged.GetUnsafeSystemRef<LocationSpawnSystem>(
                    m_LocationSpawnSystem);
            system.Inject(container);
            
            system.AddInitialization(ecb =>
            {
                foreach (var root in m_LocationManager.CurrentLocationRoots)
                {
                    root.Spawn(ecb);
                }
            });

            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<GameSpawnSystemGroup>()
                .AddSystemToUpdateList(m_LocationSpawnSystem);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<GameSpawnSystemGroup>().SortSystems();
        }

        private void OnDestroy()
        {
            World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<GameSpawnSystemGroup>()
                .RemoveSystemFromUpdateList(m_LocationSpawnSystem);
            World.DefaultGameObjectInjectionWorld?.DestroySystem(m_LocationSpawnSystem);
        }
    }
}