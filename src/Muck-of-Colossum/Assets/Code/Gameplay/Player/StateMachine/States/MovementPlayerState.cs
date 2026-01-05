using Code.Gameplay.Cameras;
using Code.Gameplay.Player.StateMachine.States.ClimbMovement;
using Code.Gameplay.Player.StateMachine.States.MovementHandler;
using Code.Gameplay.Player.StateMachine.States.RotationLogic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Gameplay.Player.StateMachine.States
{
    public abstract class MovementPlayerState : IPlayerState
    {
        protected IMovementDirectionHandler movementDirectionHandler;
        protected IRotationStrategy rotationStrategy;
    
        protected readonly IStateSwitcher StateSwitcher;
        protected readonly PlayerStateMachineData Data;
    
        protected readonly ClimbChecker climbChecker;
    
        protected RotationContext rotationContext = new RotationContext();
        protected MovementDirectionContext movementDirectionContext = new MovementDirectionContext();

        private readonly Player _player;
        private readonly CameraController playerCamera;
    
        public MovementPlayerState(IStateSwitcher stateSwitcher, PlayerStateMachineData data,
            Player player)
        {
            StateSwitcher = stateSwitcher;
            Data = data;
            this._player = player;
            playerCamera = data.CameraController;
            climbChecker = player.ClimbChecker;

            movementDirectionContext.Player = player;
            movementDirectionContext.CameraTransform = playerCamera.CurrentCamera.transform;
        }

        protected PlayerInput Input => _player.Input;
        protected CharacterController CharacterController => _player.Controller;
        protected PlayerView View => _player.View;
        public virtual void Enter()
        {
            View.StartMovement();
            //Debug.Log(GetType());
        
            CharacterController.detectCollisions = false;

            AddInputActionsCallbacks();
            rotationContext.CharacterTransform = _player.transform;
        }

        public virtual void Exit()
        {
            RemoveInputActionsCallbacks();
            View.StopMovement();
        }

        public void HandleInput()
        {
            Data.XYInput = ReadVerticalAndHorizontalInput();
            movementDirectionContext.Input = Data.XYInput;
        }

        public virtual void Update()
        {
            Vector3 velocity = GetConvertedVelocity();
            velocity *= Data.Speed;
            velocity.y = Data.YVelocity;
        
            rotationContext.MoveDirection = velocity;
        
            CharacterController.Move(velocity *Time.deltaTime);
            _player.transform.rotation = GetRotationFrom();
        
        }

        protected virtual void AddInputActionsCallbacks()
        {
            Input.Movement.Climb.started += OnClimbKeyPressed;
        }

        protected virtual void RemoveInputActionsCallbacks()
        {
            Input.Movement.Climb.started -= OnClimbKeyPressed;
        }
    
        protected bool IsInputZero() => Data.XYInput == Vector2.zero;

        protected virtual void OnClimbKeyPressed(InputAction.CallbackContext obj)
        {
            if (Data.IsClimbing == true)
            {
                return;
            }
        
            climbChecker.CheckClimb();
            if (climbChecker.IsClimbable == false)
            {
                return;
            }
        
            if(Data.XYInput.y == 0)
                StateSwitcher.SwitchState<ClimbPlayerIdleState>();
            else
                StateSwitcher.SwitchState<ClimbPlayerMoveState>();
        }
    
        private Quaternion GetRotationFrom()
        {
        
            Quaternion newRotation = rotationStrategy.GetTargetRotation(rotationContext);
        
            Quaternion targetRotation = Quaternion.Slerp(_player.transform.rotation, newRotation, 
                Data.RotationSpeed * Time.deltaTime);

            return targetRotation;
        }
    
        private Vector3 GetConvertedVelocity()
        {
            return movementDirectionHandler.GetConvertedVelocity(movementDirectionContext);
        }

        private Vector2 ReadVerticalAndHorizontalInput() => Input.Movement.Move.ReadValue<Vector2>();

    }
}
