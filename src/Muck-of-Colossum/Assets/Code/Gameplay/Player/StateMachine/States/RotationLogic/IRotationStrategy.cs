using UnityEngine;

namespace Code.Gameplay.Player.StateMachine.States.RotationLogic
{
    public interface IRotationStrategy
    {
        Quaternion GetTargetRotation(RotationContext context);
    }
}
