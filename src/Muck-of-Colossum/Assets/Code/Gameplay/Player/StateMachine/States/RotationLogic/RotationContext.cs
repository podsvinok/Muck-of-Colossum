using Code.Gameplay.Climbing_System;
using UnityEngine;

namespace Code.Gameplay.Player.StateMachine.States.RotationLogic
{
    public class RotationContext 
    {
        public Vector3 MoveDirection { get; set; }
        public Vector3 OldNormal { get; set; }
        public Vector3 NewNormal { get; set; }
        public Transform CharacterTransform { get; set; }
        public TriangleMeshHelper TriangleMeshHelper { get; set; }
        public float XInput { get; set; }
    }
}
