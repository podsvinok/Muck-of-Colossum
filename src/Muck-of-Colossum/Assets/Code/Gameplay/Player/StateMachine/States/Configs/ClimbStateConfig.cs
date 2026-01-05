using System;
using UnityEngine;

namespace Code.Gameplay.Player.StateMachine.States.Configs
{
    [Serializable]
    public class ClimbStateConfig 
    {
        [SerializeField, Range(0, 50)] 
        private float moveSpeed = 2f;
    
        [SerializeField, Range(0, 100)] 
        private float rotationSpeed = 10f;
    
        [SerializeField] 
        private float attachDistance = 2f;
    
        [SerializeField] 
        private LayerMask climbableMask;
    

        public float MoveSpeed => moveSpeed;
        public float RotationSpeed => rotationSpeed;
        public float AttachDistance => attachDistance;
        public LayerMask ClimbableMask => climbableMask;
    }
}
