using Code.Gameplay.Player.StateMachine.States.Configs;

namespace Code.Gameplay.Player.StateMachine.States.Airborne
{
    public class JumpingPlayerState : AirbornePlayerState
    {
        private JumpingStateConfig config;
    
        public JumpingPlayerState(IStateSwitcher stateSwitcher, PlayerStateMachineData data, Player player) 
            : base(stateSwitcher, data, player)
        {
            config = player.Config.AirborneStateConfig.JumpingStateConfig;
        }

        public override void Enter()
        {
            base.Enter();
        
            View.StartJumping();

            Data.YVelocity = config.StartYVelocity;
        }

        public override void Exit()
        {
            base.Exit();
        
            View.StopJumping();
        }

        public override void Update()
        {
            base.Update();
        
            if(Data.YVelocity <= 0)
                StateSwitcher.SwitchState<FallingPlayerState>();
        }
    }
}
