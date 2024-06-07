using System;

using Common.Core;

using Game.Core.Inputs;

using Reflex.Attributes;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerInputHandler : MonoBehaviour, IInitialization
    {
        [Inject] private IPlayerInputs m_PlayerInputs;

        private test_MoveComponent m_MoveComponent;
        private float _startTime;
        
        private void Awake()
        {
            m_MoveComponent = GetComponent<test_MoveComponent>();
        }

        public void Initialization(IContainer container)
        {
            m_PlayerInputs.MoveAction.started += SetMoveVector; 
            m_PlayerInputs.MoveAction.performed += SetMoveVector;
            m_PlayerInputs.MoveAction.canceled += SetMoveVector;
        }
        
        private void SetMoveVector(InputAction.CallbackContext callbackContext)
        {
            var vector = math.normalize(callbackContext.ReadValue<Vector2>());
            m_MoveComponent.SetMove(math.normalize(new float3(vector.x, 0, vector.y)));
            if (callbackContext.started)
            {
                _startTime = (float)callbackContext.startTime;
            }

            var velocity = (callbackContext.time - _startTime) * 0.02f;
            m_MoveComponent.SetVelocity((float)velocity);
            
        }
    }
}
