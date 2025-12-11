using System;
using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerView : NetworkBehaviour
{
    private const string IsMovement = "IsMovement";

    private const string IsGrounded = "IsGrounded";
    private const string IsIdle = "IsIdle";
    private const string IsRunning = "IsRunning";

    private const string IsAirborne = "IsAirborne";
    private const string IsFalling = "IsFalling";
    private const string IsJumping = "IsJumping";

    private const string IsClimbing = "IsClimbing";
    private const string IsClimbIdle = "IsClimbIdle";
    private const string IsClimbMoving = "IsClimbMoving";

    private Animator animator;

    public void Init()
    {
        animator = GetComponent<Animator>();
    }

    public void StartMovement() => animator.SetBool(IsMovement, true);
    public void StopMovement() => animator.SetBool(IsMovement, false);

    public void StartGrounded() => animator.SetBool(IsGrounded, true);
    public void StopGrounded() => animator.SetBool(IsGrounded, false);

    public void StartIdle() => animator.SetBool(IsIdle, true);
    public void StopIdle() => animator.SetBool(IsIdle, false);

    public void StartRunning() => animator.SetBool(IsRunning, true);
    public void StopRunning() => animator.SetBool(IsRunning, false);

    public void StartAirborne() => animator.SetBool(IsAirborne, true);
    public void StopAirborne() => animator.SetBool(IsAirborne, false);

    public void StartFalling() => animator.SetBool(IsFalling, true);
    public void StopFalling() => animator.SetBool(IsFalling, false);

    public void StartJumping() => animator.SetBool(IsJumping, true);
    public void StopJumping() => animator.SetBool(IsJumping, false);
    
    public void StartClimbing() => animator.SetBool(IsClimbing, true);
    public void StopClimbing() => animator.SetBool(IsClimbing, false);
    
    public void StartClimbIdle() => animator.SetBool(IsClimbIdle, true);
    public void StopClimbIdle() => animator.SetBool(IsClimbIdle, false);
    
    public void StartClimbMoving() => animator.SetBool(IsClimbMoving, true);
    public void StopClimbMoving() => animator.SetBool(IsClimbMoving, false);
}
