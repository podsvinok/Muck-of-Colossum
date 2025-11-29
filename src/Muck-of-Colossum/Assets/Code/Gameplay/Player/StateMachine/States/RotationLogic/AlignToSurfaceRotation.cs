using UnityEngine;

public class AlignToSurfaceRotation : IRotationStrategy
{
    Vector3 oldNormal;
    Vector3 newNormal;

    private Transform character;
    
    public Quaternion GetTargetRotation(RotationContext context)
    {
        
        // 1. Берём текущий forward (игрок сам им управляет)
        Vector3 desiredForward = character.forward;

        // 2. Проецируем forward на новую поверхность
        Vector3 forwardOnSurface = Vector3.ProjectOnPlane(desiredForward, newNormal).normalized;
        if (forwardOnSurface == Vector3.zero)
            forwardOnSurface = Vector3.ProjectOnPlane(character.up, newNormal).normalized;

        // 3. Собираем локальный поворот: forward + нормаль
        return Quaternion.LookRotation(forwardOnSurface, newNormal);
    }
}
