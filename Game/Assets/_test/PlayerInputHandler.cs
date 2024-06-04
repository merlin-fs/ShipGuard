using System;

using Common.Core;

using Game.Core.Inputs;

using Reflex.Attributes;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerInputHandler : MonoBehaviour, IInitialization
    {
        [Inject] private IPlayerInputs m_PlayerInputs;

        private void Awake()
        {
            
        }

        public void Initialization(IContainer container)
        {
            m_PlayerInputs.MoveAction.started += SetMoveVector; 
            m_PlayerInputs.MoveAction.performed += SetMoveVector;
        }
        
        private void SetMoveVector(InputAction.CallbackContext callbackContext)
        {
            //_movementInput.SetValueInterpolated(callbackContext.ReadValue<Vector2>());
        }
    }
}
