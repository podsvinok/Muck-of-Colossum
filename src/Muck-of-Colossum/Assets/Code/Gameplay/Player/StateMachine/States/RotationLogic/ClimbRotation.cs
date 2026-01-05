using UnityEngine;

namespace Code.Gameplay.Player.StateMachine.States.RotationLogic
{
    public class ClimbRotation : IRotationStrategy
    {
        public Quaternion GetTargetRotation(RotationContext context)
        {
            Vector3 rotationDirection = Vector3.zero;
        
            Vector3 oldNormal = context.OldNormal;
            Vector3 newNormal = context.NewNormal;
            float xInput = context.XInput;
            Transform characterTransform = context.CharacterTransform;

            if (xInput > 0)
                rotationDirection += characterTransform.right;
            else 
                rotationDirection -= characterTransform.right;

            // 1. Определяем forward: если игрок нажимает AD (накопилось), берем его
            Vector3 desiredForward = xInput != 0 
                ? rotationDirection.normalized 
                : characterTransform.forward;

            // 2. Проецируем forward на поверхность, чтобы forward не улетал в воздух
            Vector3 forwardOnSurface = Vector3.ProjectOnPlane(desiredForward, newNormal).normalized;

            // Если forward совпал с нормалью (редкий случай), берем крест к up
            if (forwardOnSurface == Vector3.zero)
                forwardOnSurface = Vector3.ProjectOnPlane(characterTransform.up, newNormal).normalized;

            // 3. Создаем ориентацию: направление туда, куда хочет игрок, и вверх по нормали поверхности
            Quaternion targetRot = Quaternion.LookRotation(forwardOnSurface, newNormal);

            return targetRot;
        }
    }
}
