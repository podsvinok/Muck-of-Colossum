using UnityEngine;

namespace Code.Gameplay.Player.StateMachine.States.MovementHandler
{
    public interface IMovementDirectionHandler
    {
        public Vector3 GetConvertedVelocity(MovementDirectionContext context);
    }
}
