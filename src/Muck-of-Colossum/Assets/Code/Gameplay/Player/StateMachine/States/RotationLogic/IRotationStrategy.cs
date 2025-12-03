using UnityEngine;

public interface IRotationStrategy
{
    Quaternion GetTargetRotation(RotationContext context);
}
