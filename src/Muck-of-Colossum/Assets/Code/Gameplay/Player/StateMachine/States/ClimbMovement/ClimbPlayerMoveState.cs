namespace Code.Gameplay.Player.StateMachine.States.ClimbMovement
{
    public class ClimbPlayerMoveState : ClimbPlayerState
    {
        public ClimbPlayerMoveState(IStateSwitcher stateSwitcher, PlayerStateMachineData data, Player player) 
            : base(stateSwitcher, data, player)
        {
        }

        protected override void OnClimbStateEnter()
        {
            View.StartClimbMoving();
        }

        public override void Exit()
        {
            base.Exit();
            View.StopClimbMoving();
        }

        public override void Update()
        {
            base.Update();

            // Переход в ClimbIdleState при остановке движения
            if (Data.XYInput.y == 0)
            {
                StateSwitcher.SwitchState<ClimbPlayerIdleState>();
            }

            // Обновляем контекст для вращения
            rotationContext.XInput = Data.XYInput.x;

            // Обрабатываем перемещение и вращение
            HandleClimbMovement();
            HandleClimbRotation();
        }
    }
}