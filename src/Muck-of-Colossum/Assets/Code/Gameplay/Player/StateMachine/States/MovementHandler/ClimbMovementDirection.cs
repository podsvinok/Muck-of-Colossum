using UnityEngine;

public class ClimbMovementDirection : IMovementDirectionHandler
{
    public Vector3 GetConvertedVelocity(MovementDirectionContext context)
    {
        Vector3 moveDirection = Vector3.zero;
        
        Transform characterTransform = context.Player.transform;
        
        float YInput = context.Input.y;

        if (YInput > 0)
            moveDirection += characterTransform.forward;
        else if (YInput < 0)
            moveDirection -= characterTransform.forward;
        
        moveDirection.Normalize();
        return moveDirection;
    }
}
