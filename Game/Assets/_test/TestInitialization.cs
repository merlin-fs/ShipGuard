using Common.Core;

using Game.Core.Camera;
using Game.Core.Repositories;
using Game.Core.Spawns;

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
        [Inject] private Spawner m_Spawner;
        [Inject] private ConfigRepository m_ConfigRepository;

        private LocationRepository m_LocationRepository;
        
        public void Initialization(IContainer container)
        {
            
            m_LocationRepository = container.Resolve<LocationRepository>();

            var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var ecb = manager.World.GetOrCreateSystemManaged<GameSpawnSystemCommandBufferSystem>()
                .CreateCommandBuffer();

            /*
            m_Spawner.Setup(ecb)
                .WithNewView()
                .WithConfigId("Player")
                .WithPosition(float3.zero)
                .WithEvent(view =>
                {
                    GetComponent<test_MoveComponent>().SetAnimator(view.Transform.GetComponent<Animator>());

                    m_CameraController.SetFollowTarget(view.Transform);
                });
                /**/
        }
    }
}
