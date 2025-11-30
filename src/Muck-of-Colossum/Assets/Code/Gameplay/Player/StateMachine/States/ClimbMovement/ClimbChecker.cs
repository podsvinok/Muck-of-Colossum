using FishNet.Object;
using UnityEngine;

public class ClimbChecker : NetworkBehaviour
{
    [SerializeField, Range(0f, 2f)] private float checkDistance;
    [SerializeField] private LayerMask climbLayer;
        
    public bool IsClimbable { get; private set; }

    private RaycastHit hitInfo;

    public void CheckClimb()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, checkDistance, climbLayer))
        {
            IsClimbable = ValidateHit(hit);
            hitInfo = hit;
        }
        else IsClimbable = false;
    }

    public bool TryGetHitInfo(out RaycastHit hitInfo)
    {
        hitInfo = this.hitInfo;
        if (IsClimbable)
        {
            return true;
        }
        else 
            return false;

    }
    
    private bool ValidateHit(RaycastHit hit)
    {
        MeshCollider targetCollider = hit.collider as MeshCollider;
        TriangleAdjacency adjacency = targetCollider.GetComponentInChildren<TriangleAdjacency>();
        
        if (adjacency == null)
            return false;
        
        return hit.collider is MeshCollider mc && mc.sharedMesh != null;
    }
}
