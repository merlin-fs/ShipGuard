using Unity.Mathematics;

using UnityEngine;

namespace Game
{
    public class test_MoveComponent : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;
        private int MovingTag { get; } = Animator.StringToHash("Moving");
        private int VelocityXTag { get; } = Animator.StringToHash("VelocityX");
        private int VelocityZTag { get; } = Animator.StringToHash("VelocityZ");
        private int VelocityTag { get; } = Animator.StringToHash("Velocity");
        

        public void SetVelocity(float velocity)
        {
            m_Animator.SetFloat(VelocityTag, velocity);
        }

        public void SetAnimator(Animator aimator)
        {
            m_Animator = aimator;
        }
        
        public void SetMove(float3 vector)
        {
            //velocityX = Vector3.Dot(shift.normalized, MovementComponent.RotateTarget.right);
            //velocityZ = Vector3.Dot(shift.normalized, MovementComponent.RotateTarget.forward);
            var isMoving = math.length(vector) > 0;
            if (isMoving)
                m_Animator.transform.rotation = quaternion.LookRotation(vector, Vector3.up);

            m_Animator.SetBool(MovingTag, isMoving);
            m_Animator.SetFloat(VelocityXTag, vector.x);
            m_Animator.SetFloat(VelocityZTag, vector.z);
        }
    }
}
