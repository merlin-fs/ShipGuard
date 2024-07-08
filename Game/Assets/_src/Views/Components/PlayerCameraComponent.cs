using Game.Core.Camera;

using Reflex.Attributes;

using UnityEngine;

namespace Game.Views
{
    public class PlayerCameraComponent :  MonoBehaviour, IViewComponent
    {
        [Inject] private PlayerInputHandler m_PlayerInputHandler;
        [Inject] private CameraController m_CameraController;
        
        public void Initialization(IView view)
        {
            m_PlayerInputHandler.SetUnitView(view);
            m_CameraController.SetFollowTarget(view.Transform);
        }
    }
}
