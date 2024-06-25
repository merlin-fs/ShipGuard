using Common.Core;

using Game.Core.Repositories;
using Game.Core.Spawns;
using Game.Model.Locations;

using Reflex.Core;

using Unity.Entities;

using UnityEngine;

namespace Game.Core.Contexts
{
    public class LocationSceneContext : MonoBehaviour, IInstaller, IInitialization
    {
        [SerializeField] private LocationRoot locationRoot;
        private SystemHandle m_LocationSpawnSystem;
        
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(locationRoot);
            containerBuilder.AddSingleton(c => c.Construct<LocationViewRepository>());
            containerBuilder.AddSingleton(c => c.Construct<LocationRepository>());
        }

        public void Initialization(IContainer container)
        {
            m_LocationSpawnSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<LocationSpawnSystem>();
            var system =
                World.DefaultGameObjectInjectionWorld.Unmanaged.GetUnsafeSystemRef<LocationSpawnSystem>(
                    m_LocationSpawnSystem);
            system.Inject(container);
            
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<GameSpawnSystemGroup>()
                .AddSystemToUpdateList(m_LocationSpawnSystem);
            World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<GameSpawnSystemGroup>().SortSystems();
        }

        private void OnDestroy()
        {
            World.DefaultGameObjectInjectionWorld?.DestroySystem(m_LocationSpawnSystem);
        }
    }
}