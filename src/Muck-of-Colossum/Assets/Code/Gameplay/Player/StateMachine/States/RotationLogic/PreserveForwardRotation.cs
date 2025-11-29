using UnityEngine;

public class PreserveForwardRotation : IRotationStrategy
{
    Vector3 oldNormal;
    Vector3 newNormal;

    private Transform character;
    
    public Quaternion GetTargetRotation(RotationContext context)
    {
        oldNormal = context.OldNormal;
        newNormal = context.NewNormal;
        character = context.CharacterTransform.transform;
        
        Vector3 forward = character.forward;
        Vector3 projectedForward = Vector3.ProjectOnPlane(forward, newNormal).normalized;

        if (projectedForward.sqrMagnitude < 0.0001f)
            projectedForward = Vector3.Cross(newNormal, character.right);

        return Quaternion.LookRotation(projectedForward, newNormal);
    }
}
