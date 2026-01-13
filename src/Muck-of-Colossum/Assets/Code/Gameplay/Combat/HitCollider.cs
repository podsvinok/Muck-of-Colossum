using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class HitCollider : MonoBehaviour
{
    public bool IsAttacking;
    
    [SerializeField] protected LayerMask targetLayer;
    protected List<Collider> hitTargets = new List<Collider>();
    public List<Collider> HitTargets => hitTargets;
    
    public event Action<RaycastHit> OnHit;

    public abstract void Init();
    
    public void ResetHits() => hitTargets.Clear();
    
    protected bool AlreadyHit(Collider target) 
        => hitTargets.Contains(target);
    
    public abstract void CheckHits();
    
    protected void InvokeHit(RaycastHit target)
    {
        OnHit?.Invoke(target);
    }
}
