using System;
using System.Linq;

using Common.Core;

using Game.Core.Inputs;
using Game.Views;

using Reflex.Attributes;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerInputHandler : MonoBehaviour, IInitialization
    {
        [Inject] private IPlayerInputs m_PlayerInputs;

        private ViewMoveComponent m_MoveComponent;
        private float _startTime;
        
        public void Initialization(IContainer container)
        {
            m_PlayerInputs.MoveAction.started += SetMoveVector; 
            m_PlayerInputs.MoveAction.performed += SetMoveVector;
            m_PlayerInputs.MoveAction.canceled += SetMoveVector;
        }

        public void SetUnitView(IView view)
        {
            m_MoveComponent = view.GetComponents<ViewMoveComponent>().First();
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
