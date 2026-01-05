using UnityEngine;

namespace Code.Gameplay.Player.StateMachine.States.MovementHandler
{
    public class StandartMovementDirection : IMovementDirectionHandler
    {
        public Vector3 GetConvertedVelocity(MovementDirectionContext context)
        {
            Vector2 input = context.Input;
            Transform cameraTransform = context.CameraTransform;
        
            Vector3 moveDirection = cameraTransform.forward * input.y;
            moveDirection += cameraTransform.right * input.x;
            moveDirection.Normalize();
            moveDirection.y = 0;
        
            return moveDirection;
        }
    }
}
