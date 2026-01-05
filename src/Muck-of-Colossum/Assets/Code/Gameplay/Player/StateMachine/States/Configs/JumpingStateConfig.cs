using System;
using UnityEngine;

namespace Code.Gameplay.Player.StateMachine.States.Configs
{
    [Serializable]
    public class JumpingStateConfig
    {
        [SerializeField, Range(0, 20)] private float maxJumpHeight;
        [SerializeField, Range(0, 20)] private float timeToReachMaxHeight;
    
        public float StartYVelocity => 2 * maxJumpHeight / timeToReachMaxHeight;
        public float MaxJumpHeight => maxJumpHeight;
        public float TimeToReachMaxHeight => timeToReachMaxHeight;
    }
}
