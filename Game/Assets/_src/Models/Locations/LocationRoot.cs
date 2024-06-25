using System;

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
        [Inject] private ConfigRepository m_ConfigRepository;
        [Inject] private LocationViewRepository m_LocationViewRepository;
        [Inject] private Spawner m_Spawner;

        private void Awake()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var ecb = entityManager.World
                .GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                .CreateCommandBuffer();
            
            foreach (var iter in GetComponentsInChildren<ViewLocation>(true))
            {
                var identifiable = iter.GetComponent<ViewComponentIdentifiable>();
                m_LocationViewRepository.Insert(identifiable.ID, iter);
               /*
                m_Spawner.Setup(ecb)
                    .WithId(identifiable.ID)
                    .WithConfigId(iter.Config)
                    .WithView(iter);
                    /**/
            }
        }

        public void Initialization(IContainer container)
        {
        }
    }
}
