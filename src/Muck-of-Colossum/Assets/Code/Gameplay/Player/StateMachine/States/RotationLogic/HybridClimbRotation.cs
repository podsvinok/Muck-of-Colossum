using UnityEngine;

public class HybridClimbRotation : IRotationStrategy
{
    private readonly Transform camera;

    private Vector3 newNormal;
    private Vector3 oldNormal;

    public HybridClimbRotation(Transform climbCamera)
    {
        camera = climbCamera;
    }

    public Quaternion GetTargetRotation(RotationContext context)
    {
        newNormal = context.NewNormal;
        oldNormal = context.OldNormal;
        
        // 1. Ап вектор — нормаль треугольника
        Vector3 up = newNormal.normalized;

        // 2. Вычисляем направление "вверх" по поверхности:
        // Глобальный вверх (Vector3.up) или направление камеры (если смотреть вертикально)
        Vector3 refDir = Vector3.up;
        // Проекция на плоскость поверхности
        Vector3 forward = Vector3.ProjectOnPlane(refDir, up).normalized;
        if (forward.sqrMagnitude < 0.001f)
            forward = Vector3.ProjectOnPlane(Vector3.up, up).normalized;

        // 3. Право вычисляется автоматически
        Vector3 right = Vector3.Cross(up, forward).normalized;

        // 4. Пересобрать форвард заново (ортонормализация)
        forward = Vector3.Cross(right, up).normalized;

        return Quaternion.LookRotation(forward, up);
    }
}
