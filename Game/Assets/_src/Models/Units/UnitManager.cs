using Common.Core;

using Game.Core.Repositories;
using Game.Core.Spawns;

using Reflex.Attributes;

using Unity.Entities;

namespace Game.Model.Units
{
    public class UnitManager
    {
        [Inject] private GameEntityRepository m_GameEntityRepository;
        [Inject] private ConfigRepository m_ConfigRepository;
        [Inject] private Spawner m_Spawner;
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
            //*
            m_Spawner.Setup(ecb)
                //.WhereCondition(gameEntity => gameEntity.)
                .WithNewView()
                .WithConfig(playerConfig)
                .WithLocationPoint(spawnPointId);
            //.WithEvent(gameEntity => )

            /**/
        }
    }
}
