using UnityEngine;

namespace Code.Gameplay.Player.StateMachine.States.RotationLogic
{
    public class StandartRotationStrategy : IRotationStrategy
    {
        public Quaternion GetTargetRotation(RotationContext context)
        {
            Vector3 moveDirection = context.MoveDirection;
            Transform characterTransform = context.CharacterTransform;
        
            Vector3 targetRotationDirection = moveDirection;
            targetRotationDirection.y = 0;
        
            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = characterTransform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        
            return newRotation;
        }
    }
}
