using UnityEngine;

public class ClimbPlayerIdleState : ClimbPlayerState
{
    public ClimbPlayerIdleState(IStateSwitcher stateSwitcher, PlayerStateMachineData data, Player player) 
        : base(stateSwitcher, data, player)
    {
    }

    protected override void OnClimbStateEnter()
    {
        View.StartClimbIdle();
        // Сбрасываем контекст движения при переходе в idle
        movementDirectionContext.Input = Vector2.zero;
    }

    public override void Exit()
    {
        base.Exit();
        View.StopClimbIdle();
    }

    public override void Update()
    {
        base.Update();

        // Поддерживаем прикрепление к поверхности
        MaintainSurfaceAttachment();

        // Переход в ClimbMoveState при начале движения
        if (Data.XYInput.y != 0)
        {
            StateSwitcher.SwitchState<ClimbPlayerMoveState>();
        }
    }
}