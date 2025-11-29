using UnityEngine;
using UnityEngine.InputSystem;

public abstract class GroundedPlayerState : MovementPlayerState
{
    private readonly GroundChecker groundChecker;
    
    public GroundedPlayerState(IStateSwitcher stateSwitcher, PlayerStateMachineData data, Player player) : base(stateSwitcher, data, player)
    {
        groundChecker = player.GroundChecker;
    }

    public override void Enter()
    {
        base.Enter();
        
        View.StartGrounded();

        movementDirectionHandler = new StandartMovementDirection();
        rotationStrategy = new StandartRotationStrategy();
    }

    public override void Exit()
    {
        base.Exit();
        
        View.StopGrounded();
        
        
    }

    public override void Update()
    {
        base.Update();

        if (groundChecker.IsTouches == false)
            StateSwitcher.SwitchState<FallingPlayerState>();
    }

    protected override void AddInputActionsCallbacks()
    {
        base.AddInputActionsCallbacks();

        Input.Movement.Jump.started += OnJumpKeyPressed;
    }
    

    protected override void RemoveInputActionsCallbacks()
    {
        base.RemoveInputActionsCallbacks();
        
        Input.Movement.Jump.started -= OnJumpKeyPressed;

    }
    
    private void OnJumpKeyPressed(InputAction.CallbackContext obj)
    {
        StateSwitcher.SwitchState<JumpingPlayerState>();
    }
}
