using UnityEngine;

public class FallingPlayerState : AirbornePlayerState
{
    private readonly GroundChecker groundChecker;
    
    public FallingPlayerState(IStateSwitcher stateSwitcher, PlayerStateMachineData data, Player player)
        : base(stateSwitcher, data, player)
    {
        groundChecker = player.GroundChecker;
    }

    public override void Enter()
    {
        base.Enter();
        
        View.StartFalling();
    }

    public override void Exit()
    {
        base.Exit();
        
        View.StopFalling();
    }

    public override void Update()
    {
        base.Update();

        if (groundChecker.IsTouches)
        {
            Data.YVelocity = 0;
            
            if(IsInputZero())
                StateSwitcher.SwitchState<GroundIdlePlayerState>();
            else
                StateSwitcher.SwitchState<RunningPlayerState>();
        }
    }
}
