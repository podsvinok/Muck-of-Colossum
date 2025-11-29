using UnityEngine;

public class RunningPlayerState : GroundedPlayerState
{
    private RunningStateConfig config;
    public RunningPlayerState(IStateSwitcher stateSwitcher, PlayerStateMachineData data, Player player) :
        base(stateSwitcher, data,  player)
    {
        config = player.Config.RunningStateConfig;
    }

    public override void Enter()
    {
        base.Enter();
        
        View.StartRunning();

        Data.Speed = config.Speed;
        Data.RotationSpeed = config.RotationSpeed;
    }

    public override void Exit()
    {
        base.Exit();
        
        View.StopRunning();
    }

    public override void Update()
    {
        base.Update();
        
        if(IsInputZero())
            StateSwitcher.SwitchState<GroundIdlePlayerState>();
    }
}
