using UnityEngine;

public interface IMovementDirectionHandler
{
    public Vector3 GetConvertedVelocity(MovementDirectionContext context);
}
