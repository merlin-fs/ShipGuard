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

        private GameUniqueEntityRepository m_GameUniqueEntityRepository;
        
        public void Initialization(IContainer container)
        {
            m_GameUniqueEntityRepository = container.Resolve<GameUniqueEntityRepository>();

            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var ecb = manager.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                .CreateCommandBuffer();

            var playerConfig = m_ConfigRepository.FindByID("Player");

            using var _ = Spawner.Setup(ecb, playerConfig)
                .WithPosition(float3.zero);
        }
    }
}
