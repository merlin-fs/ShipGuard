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
        [Inject] private LocationViewRepository m_LocationViewRepository;
        [Inject] private ConfigRepository m_ConfigRepository;
        [Inject] private Spawner m_Spawner;

        public void Spawn(EntityCommandBuffer ecb)
        {
            foreach (var iter in GetComponentsInChildren<ViewLocation>(true))
            {
                var identifiable = iter.GetComponent<ViewComponentIdentifiable>();
                m_LocationViewRepository.Insert(identifiable.ID, iter);
                var config = m_ConfigRepository.FindByID(iter.ConfigId);

                m_Spawner.Setup(ecb)
                    .WhereCondition(gameEntity => !m_LocationViewRepository.FindByID(gameEntity.ID)!.HasInitialize)
                    .WithId(identifiable.ID)
                    .WithConfig(config)
                    .WithView(iter);
                /**/
            }
        }
        
        public void Initialization(IContainer container)
        {
        }
    }
}
