using System;
using System.Text;

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
        [Inject] private GameUniqueEntityRepository m_GameUniqueEntityRepository;
        [Inject] private ConfigRepository m_ConfigRepository;
        private EntityManager m_EntityManager;

        private Uuid m_PlayerUid;
        
        
        public UnitManager()
        {
            m_EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            byte[] bytes = Encoding.ASCII.GetBytes("Player - 1234567");
            sbyte[] signed = (sbyte[]) (Array) bytes;
            m_PlayerUid = Uuid.FromByte(signed);
        }


        public void SpawnMobs(EntityCommandBuffer ecb, Entity spawnPointEntity, ObjectID configId, int count)
        {
            var config = m_ConfigRepository.FindByID(configId);
            Spawner.Setup(ecb, config, count, spawner =>
            {
                spawner
                    .WithLocationPoint(spawnPointEntity)
                    .WithStorage<StorageLocation>();
            });
        }
        
        public GameEntity? GetPlayer()
        {
            return null;
        }

        public void SpawnPlayer(Uuid spawnPointId)
        {
            var playerConfig = m_ConfigRepository.FindByID("Player");
            if (m_GameUniqueEntityRepository.FindByID(m_PlayerUid) != null) return;

            var spawnPointEntity = m_GameUniqueEntityRepository.FindByID(spawnPointId);
            
            var ecb = m_EntityManager.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                .CreateCommandBuffer();
            
            using var _ = Spawner.SetupUnique(ecb, playerConfig, m_PlayerUid)
                .WithStorage<StorageLocation>()
                .WithLocationPoint(spawnPointEntity.Entity);
        }

        public void DestroyPlayer()
        {
            var ecb = m_EntityManager.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                .CreateCommandBuffer();

            var playerConfig = m_ConfigRepository.FindByID("Player") as UnitConfig;
            Spawner.Destroy(ecb, m_GameUniqueEntityRepository.FindByID(m_PlayerUid));
        }
    }
}
