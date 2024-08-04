using System;
using System.Linq;
using System.Threading.Tasks;

using Common.Core.Loading;

using Cysharp.Threading.Tasks;

using Game.Core.Repositories;
using Game.Model;
using Game.Model.Locations;
using Game.Model.Units;
using Game.Views;

using Reflex.Attributes;

using Unity.Entities;

namespace Game.Core.Loading
{
    public class CommandSpawnPlayer : ICommand
    {
        [Inject] private UnitManager m_UnitManager;
        [Inject] private LocationViewRepository m_LocationViewRepository;
        [Inject] private GameUniqueEntityRepository m_GameUniqueEntityRepository;

        public Task Execute()
        {
            return UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();

                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager.WorldUnmanaged.EntityManager;
                var locationPointGameEntity = m_GameUniqueEntityRepository.Find(iter => entityManager.HasComponent<PlayerSpawnPoint>(iter.Entity.Entity)).First();
                var uniqueEntity = entityManager.GetComponentData<UniqueEntity>(locationPointGameEntity.Entity);
                var locationPointId = uniqueEntity.ID;
                
                m_UnitManager.SpawnPlayer(locationPointId);
            }).AsTask();
        }
    }
}