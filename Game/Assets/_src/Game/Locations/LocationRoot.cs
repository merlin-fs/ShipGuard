using Common.Core;

using Game.Core.Repositories;
using Game.Core.Spawns;
using Game.Storages;
using Game.Views;

using Reflex.Attributes;

using Unity.Entities;

using UnityEngine;

namespace Game.Model.Locations
{
    public class LocationRoot : MonoBehaviour, IInitialization
    {
        [Inject] private LocationViewRepository m_LocationViewRepository;
        [Inject] private GameUniqueEntityRepository m_GameUniqueEntityRepository;
        [Inject] private ConfigRepository m_ConfigRepository;

        public void SpawnLocationItems(EntityCommandBuffer ecb)
        {
            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            foreach (var iter in GetComponentsInChildren<ViewLocation>(true))
            {
                var identifiable = iter.GetComponent<ViewComponentIdentifiable>();
                m_LocationViewRepository.Insert(identifiable.ID, iter);
                if (m_GameUniqueEntityRepository.FindByID(identifiable.ID) != null) 
                    continue;
                
                var configuration = iter.GetComponent<SetupConfigComponent>();
                var config = m_ConfigRepository.FindByID(configuration.ConfigId);

                using var _ = Spawner.SetupUnique(ecb, config, identifiable.ID)
                    .WithStorage<StorageLocation>()
                    .WithCallback(spawnPointEntity =>
                    {
                        configuration.SetupPrepareData(ecb, spawnPointEntity);
                    });
            }
        }

        public void DestroyLocationItems(EntityCommandBuffer ecb)
        {
            foreach (var iter in m_LocationViewRepository.Find())
            {
                if (iter is not ViewLocation viewLocation) continue;
                Spawner.DestroyEntityOnly(ecb, viewLocation.GameEntity);
            }
        }

        public void Initialization(IContainer container)
        {
        }
    }
}
