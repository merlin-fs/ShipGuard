using Common.Core;

using Game.Core.Camera;
using Game.Core.Repositories;
using Game.Core.Spawns;
using Game.Model;

using Reflex.Attributes;

using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

namespace Game
{
    public class TestInitialization : MonoBehaviour, IInitialization
    {
        [SerializeField] private Transform target;

        [Inject] private CameraController m_CameraController;
        [Inject] private ConfigRepository m_ConfigRepository;

        private GameEntityRepository m_GameEntityRepository;
        
        public void Initialization(IContainer container)
        {
            m_GameEntityRepository = container.Resolve<GameEntityRepository>();

            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var ecb = manager.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                .CreateCommandBuffer();

            var playerConfig = m_ConfigRepository.FindByID("Player");

            using var _ = Spawner.Setup(ecb, playerConfig)
                .WithNewView()
                .WithPosition(float3.zero);
        }
    }
}
