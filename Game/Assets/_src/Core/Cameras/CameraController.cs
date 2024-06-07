using Unity.Cinemachine;

using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Core.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera cinemachineCamera;
        
        
        public void SetFollowTarget(Transform value)
        {
            cinemachineCamera.Follow = value;
            //ResetFocusTarget();
            /*
            if (_generalConfigStorage.GetParam(OffsetByXKey).BoolValue)
            {
                _getUserUnit.Invoke().GetComponent<UnitNavMeshMoveComponent>().OnUnitMove.AddListener(OnPlayerMove);
                _cinemachineFraming = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                _startXOffset = _cinemachineFraming.m_ScreenX;
                _isOffsetActive = true;
                _signalBus.Subscribe<PlayerInputLockSignal>(a => ActivateCameraOffset(!a.IsLock));
            }
            */
        }
    }
}
