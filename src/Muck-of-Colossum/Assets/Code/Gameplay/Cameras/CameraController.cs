
using FishNet.Object;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera currentCamera;
    [SerializeField] private Transform followTarget;
    public CinemachineCamera CurrentCamera => currentCamera;

    public override void OnStartClient()
    {
        Init();
        currentCamera.enabled = IsOwner;
    }
    
    public void Init()
    {
        currentCamera = GetComponentInChildren<CinemachineCamera>();
    }
}
