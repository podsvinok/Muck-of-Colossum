using UnityEngine;

public class AlignToNormalRotation : IRotationStrategy
{
    public Quaternion GetTargetRotation(RotationContext context)
    {
        Vector3 oldNormal = context.OldNormal;
        Vector3 newNormal = context.NewNormal;
        Transform character = context.CharacterTransform.transform;
        
        Quaternion align = Quaternion.FromToRotation(oldNormal, newNormal);
        Vector3 newForward = align * character.forward;

        return Quaternion.LookRotation(
            Vector3.ProjectOnPlane(newForward, newNormal).normalized,
            newNormal
        );
    }
}
