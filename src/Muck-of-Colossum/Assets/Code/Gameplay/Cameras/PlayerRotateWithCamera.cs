using Code.Gameplay.Levels;
using FishNet.Object;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class PlayerRotateWithCamera : NetworkBehaviour
{
    private Transform cameraTransform;
    public float rotationSpeed = 10f;
    public CinemachineCamera cinemachineCamera;

    private ILevelDataProvider levelData;
    
    [Inject]
    public void Construct(ILevelDataProvider levelData)
    {
        this.levelData = levelData;
    }

    public override void OnStartClient()
    {
        if (!IsOwner)
            cinemachineCamera.enabled = false;
        else
        {
            cameraTransform = levelData.Camera;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (!IsOwner)
            return;
        
        Vector3 lookDir = cameraTransform.forward;
        lookDir.y = 0; // Keep upright

        Quaternion targetRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }
}