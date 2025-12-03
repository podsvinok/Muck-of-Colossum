using UnityEngine;

public class GroundIdlePlayerState : GroundedPlayerState
{
    public GroundIdlePlayerState(IStateSwitcher stateSwitcher, PlayerStateMachineData data, Player player) :
        base(stateSwitcher, data, player)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        
        View.StartIdle();
    }

    public override void Exit()
    {
        base.Exit();
        
        View.StopIdle();
    }

    public override void Update()
    {
        base.Update();
        if (IsInputZero())
            return;
        
        StateSwitcher.SwitchState<RunningPlayerState>();
    }
}
