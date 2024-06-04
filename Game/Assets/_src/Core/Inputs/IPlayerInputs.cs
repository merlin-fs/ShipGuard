using UnityEngine.InputSystem;

namespace Game.Core.Inputs
{
    public interface IPlayerInputs
    {
        InputAction MoveAction { get; }
    }
}
