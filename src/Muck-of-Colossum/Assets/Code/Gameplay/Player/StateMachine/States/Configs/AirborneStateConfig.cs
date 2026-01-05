using System;
using UnityEngine;

namespace Code.Gameplay.Player.StateMachine.States.Configs
{
    [Serializable]
    public class AirborneStateConfig
    {
        [SerializeField, Range(0, 50)] private float speed;
        [SerializeField] private JumpingStateConfig jumpingStateConfig;
    
        public JumpingStateConfig JumpingStateConfig  => jumpingStateConfig;
        public float Speed => speed;
    
        public float BaseGravity 
            => 2f * jumpingStateConfig.MaxJumpHeight /
               (jumpingStateConfig.TimeToReachMaxHeight * jumpingStateConfig.TimeToReachMaxHeight); 
    }
}
