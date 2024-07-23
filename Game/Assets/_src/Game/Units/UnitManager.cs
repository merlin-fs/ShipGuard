using Common.Core;

using Game.Core.Repositories;
using Game.Core.Spawns;
using Game.Storages;

using Reflex.Attributes;

using Unity.Entities;

namespace Game.Model.Units
{
    public class UnitManager
    {
        [Inject] private GameEntityRepository m_GameEntityRepository;
        [Inject] private ConfigRepository m_ConfigRepository;
        private EntityManager m_EntityManager;

        public UnitManager()
        {
            m_EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        
        public GameEntity? GetPlayer()
        {
            return null;
        }

        public void SpawnPlayer(Uuid spawnPointId)
        {
            var ecb = m_EntityManager.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                .CreateCommandBuffer();

            var playerConfig = m_ConfigRepository.FindByID("Player");
            using var _ = Spawner.Setup(ecb, playerConfig)
                .WhereCondition(gameEntity => m_GameEntityRepository.FindByID(gameEntity.ID) == null)
                .WithStorage<StorageUser>()
                .WithNewView()
                .WithLocationPoint(spawnPointId);
        }

        public void DestroyPlayer()
        {
            var ecb = m_EntityManager.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                .CreateCommandBuffer();

            var playerConfig = m_ConfigRepository.FindByID("Player") as UnitConfig;
            Spawner.Destroy(ecb, m_GameEntityRepository.FindByID(playerConfig.UnitID));
        }
    }
}
