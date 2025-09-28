using FishNet.Object;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.Gameplay.Cameras
{
    public class PlayerCamera : NetworkBehaviour
    {
        [SerializeField] private new CinemachineCamera camera;

        public override void OnStartClient()
        {
            camera.enabled = IsOwner;
        }
    }
}