using Code.Gameplay.Player.StateMachine.States.Configs;
using Code.Gameplay.Player.StateMachine.States.MovementHandler;
using Code.Gameplay.Player.StateMachine.States.RotationLogic;
using UnityEngine;

namespace Code.Gameplay.Player.StateMachine.States.Airborne
{
    public abstract class AirbornePlayerState : MovementPlayerState
    {
        private readonly AirborneStateConfig airborneStateConfig;
        public AirbornePlayerState(IStateSwitcher stateSwitcher, PlayerStateMachineData data, Player player)
            : base(stateSwitcher, data, player)
        {
            airborneStateConfig = player.Config.AirborneStateConfig;
        }

        public override void Enter()
        {
            base.Enter();
        
            View.StartAirborne();
        
            movementDirectionHandler = new StandartMovementDirection();
            rotationStrategy = new StandartRotationStrategy();
        
            Data.Speed = airborneStateConfig.Speed;
        }

        public override void Exit()
        {
            base.Exit();
        
            View.StopAirborne();
        }

        public override void Update()
        {
            base.Update();
        
            Data.YVelocity -= airborneStateConfig.BaseGravity * Time.deltaTime;
        }

        protected virtual float GetGravityMultiplier() => airborneStateConfig.BaseGravity;
    }
}
