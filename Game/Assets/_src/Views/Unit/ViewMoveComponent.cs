using Game.Core.HybridTransforms;
using Game.Views;

using Unity.Entities;
using Unity.Mathematics;

using UnityEngine;

namespace Game
{
    public class ViewMoveComponent : MonoBehaviour, IViewComponent
    {
        [SerializeField] private Animator m_Animator;
        #region StringToHash
        private int MovingTag { get; } = Animator.StringToHash("Moving");
        private int VelocityXTag { get; } = Animator.StringToHash("VelocityX");
        private int VelocityZTag { get; } = Animator.StringToHash("VelocityZ");
        private int VelocityTag { get; } = Animator.StringToHash("Velocity");
        #endregion

        private EntityCommandBufferSystem m_EcbSystem;
        private Entity m_Entity;
        
        public void SetVelocity(float velocity)
        {
            m_Animator.SetFloat(VelocityTag, velocity);
        }

        public void SetMove(float3 vector)
        {
            //velocityX = Vector3.Dot(shift.normalized, MovementComponent.RotateTarget.right);
            //velocityZ = Vector3.Dot(shift.normalized, MovementComponent.RotateTarget.forward);
            var isMoving = math.length(vector) > 0;
            if (isMoving)
            {
                m_Animator.transform.rotation = quaternion.LookRotation(vector, Vector3.up);
                m_EcbSystem.CreateCommandBuffer()
                    .AddComponent<HybridTransform.ViewToEntityTag>(m_Entity);
            }
            else
            {
                m_EcbSystem.CreateCommandBuffer()
                    .RemoveComponent<HybridTransform.ViewToEntityTag>(m_Entity);
            }

            m_Animator.SetBool(MovingTag, isMoving);
            m_Animator.SetFloat(VelocityXTag, vector.x);
            m_Animator.SetFloat(VelocityZTag, vector.z);
            
        }

        public void Initialization(IView view)
        {
            m_Entity = view.GameEntity.Entity;
            m_EcbSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EndSimulationEntityCommandBufferSystem>();
        }
    }
}
