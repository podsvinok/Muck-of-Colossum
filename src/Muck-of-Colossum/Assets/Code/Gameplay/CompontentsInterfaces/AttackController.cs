
using FishNet.Object;
using UnityEngine;

public class AttackController : NetworkBehaviour, IAttack
{
    [SerializeField] HitCollider hitCollider;

    private float baseDamage;

    public void Init(float baseDamage)
    {
        this.baseDamage = baseDamage;
    }
    
    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        hitCollider.OnHit += HandleHit;
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        hitCollider.OnHit -= HandleHit;
    }


    private void HandleHit(RaycastHit hit)
    {
        if (!IsOwner && !IsServer) return;
        
        GameObject targetObj = hit.collider.gameObject;
        CmdApplyDamage(targetObj, hit.point);
    }
    
    public void Attack()
    {
        
    }

    public float CalculateDamage()
    {
        return baseDamage;
    }
    public void ApplyDamage(Vector3 hitPoint, IHealth health, float damage)
    {
        health.TakeDamage(damage, hitPoint);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void CmdApplyDamage(GameObject target, Vector3 hitPoint)
    {
        Debug.Log("Try Get IHealth");
        if (target != null && target.TryGetComponent<IHealth>(out var health))
        {
            float damage = CalculateDamage();
            ApplyDamage(hitPoint, health, damage);
            Debug.Log("Damage Applied");
        }
    }
    

    public void OpenCollider()
    {
        hitCollider.IsAttacking = true;
    }

    public void CloseCollider()
    {
        hitCollider.IsAttacking = false;
        hitCollider.ResetHits();
    }
}
