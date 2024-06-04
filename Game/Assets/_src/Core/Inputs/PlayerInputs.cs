using UnityEngine.InputSystem;

namespace Game.Core.Inputs
{
    public class PlayerInputs : IPlayerInputs
    {
        public InputAction MoveAction { get; }

        public PlayerInputs(InputActionAsset asset)
        {
            var character = asset.FindActionMap("Character", throwIfNotFound: true);
            character.Enable();
            MoveAction = character.FindAction("Move", throwIfNotFound: true);
        }
    }
}
