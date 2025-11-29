using UnityEngine;

public class StandartMovementDirection : IMovementDirectionHandler
{
    public Vector3 GetConvertedVelocity(MovementDirectionContext context)
    {
        Vector2 input = context.Input;
        Camera camera = context.Camera;
        
        Vector3 moveDirection = camera.transform.forward * input.y;
        moveDirection += camera.transform.right * input.x;
        moveDirection.Normalize();
        moveDirection.y = 0f;
        
        return moveDirection;
    }
}
