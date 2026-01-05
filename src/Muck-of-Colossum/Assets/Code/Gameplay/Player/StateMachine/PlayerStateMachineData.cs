using System;
using Code.Gameplay.Cameras;
using Code.Gameplay.Climbing_System;
using UnityEngine;

namespace Code.Gameplay.Player.StateMachine
{
    public class PlayerStateMachineData 
    {
        public float XVelocity;
        public float YVelocity;
        public float ZVelocity;

        private float speed;
        private float rotationSpeed;
        private Vector2 xYInput = Vector2.zero;

        private CameraController cameraController;
    
        // === Данные для состояния лазания ===
        public MeshCollider ClimbTargetCollider { get; set; }
        public Mesh ClimbTargetMesh { get; set; }
        public int ClimbTriangleIndex { get; set; } = -1;
        public Vector3 ClimbBarycentricCoords { get; set; }
        public SurfaceNavigator ClimbNavigator { get; set; }
        public bool IsClimbing { get; set; } = false;
    
        public CameraController CameraController { get => cameraController; set => cameraController = value; }
        public Vector2 XYInput
        {
            get { return xYInput; }
            set
            {
                if ((xYInput.x < -1 || xYInput.x > 1) || (xYInput.y < -1 || xYInput.y > 1))
                    throw new ArgumentOutOfRangeException(nameof(value));
            
                xYInput = value;
            }
        }

        public float Speed
        {
            get { return speed; }

            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                speed = value;
            }
        }

        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set
            {
                if(value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                rotationSpeed = value;
            }
        }
    }
}
