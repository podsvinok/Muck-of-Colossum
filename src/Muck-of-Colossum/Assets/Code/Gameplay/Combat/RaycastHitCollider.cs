using System.Collections.Generic;
using UnityEngine;

public class RaycastHitCollider : HitCollider
{
    [SerializeField] private List<Transform> weaponColliderPoints;
    [SerializeField] private float radius;

    private List<Vector3> previousColliderPoints = new List<Vector3>();

    public override void Init()
    {
        foreach (var weaponPoint in weaponColliderPoints)
        {
            previousColliderPoints.Add(weaponPoint.position);
        }
    }

    public override void CheckHits()
    {
        for (int i = 0; i < weaponColliderPoints.Count; i++)
        {
            Vector3 previousPoint = previousColliderPoints[i];
            Vector3 actualPoint = weaponColliderPoints[i].position;
            float distance = Vector3.Distance(previousPoint, actualPoint);
            Vector3 direction = actualPoint - previousPoint;
            direction.Normalize();

            if (Physics.SphereCast(previousPoint, radius, direction,
                    out RaycastHit hit, distance, targetLayer))
            {
                if (AlreadyHit(hit.collider))
                    continue;
                
                hitTargets.Add(hit.collider);
                
                InvokeHit(hit);
            }
            
            previousColliderPoints[i] = actualPoint;
        }
       
    }

    private void FixedUpdate()
    {
        if (IsAttacking)
        {
            CheckHits();
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (weaponColliderPoints == null || weaponColliderPoints.Count == 0)
            return;
    
        Gizmos.color = IsAttacking ? Color.red : Color.green;
    
        foreach (var point in weaponColliderPoints)
        {
            if (point != null)
            {
                Gizmos.DrawWireSphere(point.position, radius);
            }
        }
    
        // Визуализация raycast между точками
        if (Application.isPlaying && previousColliderPoints != null)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < weaponColliderPoints.Count && i < previousColliderPoints.Count; i++)
            {
                if (weaponColliderPoints[i] != null)
                {
                    Gizmos.DrawLine(previousColliderPoints[i], weaponColliderPoints[i].position);
                    Gizmos.DrawWireSphere(previousColliderPoints[i], radius * 0.5f);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        
        if (weaponColliderPoints == null) return;
    
        UnityEditor.Handles.color = new Color(1, 0, 0, 0.1f);
        foreach (var point in weaponColliderPoints)
        {
            if (point != null)
            {
                UnityEditor.Handles.SphereHandleCap(0, point.position, Quaternion.identity, radius * 2, EventType.Repaint);
            }
        }
    }
#endif
}
