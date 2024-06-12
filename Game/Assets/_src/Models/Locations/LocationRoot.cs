using Common.Core;

using Game.Core.Repositories;
using Game.Core.Spawns;
using Game.Views;

using Reflex.Attributes;

using Unity.Entities;

using UnityEngine;

namespace Game.Model.Locations
{
    public class LocationRoot : MonoBehaviour, IInitialization
    {
        [Inject] private Spawner m_Spawner; 
        [Inject] private ObjectRepository m_ObjectRepository;
        
        public void Initialization(IContainer container)
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var ecb = manager.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                .CreateCommandBuffer();
            foreach (var iter in GetComponentsInChildren<ViewLocation>(true))
            {
                var config = m_ObjectRepository.FindByID(iter.Config);

                m_Spawner.Spawn(config, ecb)
                    .WithView(iter);
            }
        }
    }
}
