
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
        if (IsOwner)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
            currentCamera.gameObject.SetActive(false);
    }
}
