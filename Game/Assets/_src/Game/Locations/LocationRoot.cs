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
        [Inject] private GameEntityRepository m_GameEntityRepository;
        [Inject] private ConfigRepository m_ConfigRepository;

        public void SpawnLocationItems(EntityCommandBuffer ecb)
        {
            foreach (var iter in GetComponentsInChildren<ViewLocation>(true))
            {
                var identifiable = iter.GetComponent<ViewComponentIdentifiable>();
                var configuration = iter.GetComponent<SetupConfigComponent>();
                
                m_LocationViewRepository.Insert(identifiable.ID, iter);
                var config = m_ConfigRepository.FindByID(configuration.ConfigId);

                using var _ = Spawner.Setup(ecb, config)
                    .WhereCondition(gameEntity => m_GameEntityRepository.FindByID(gameEntity.ID) == null)
                    .WithId(identifiable.ID)
                    .WithStorage<StorageLocation>();
            }
        }

        public void DestroyLocationItems(EntityCommandBuffer ecb)
        {
            foreach (var iter in m_LocationViewRepository.Find())
            {
                if (iter is not ViewLocation viewLocation) continue;
                Spawner.Destroy(ecb, viewLocation.GameEntity);
            }
        }

        public void Initialization(IContainer container)
        {
        }
    }
}
