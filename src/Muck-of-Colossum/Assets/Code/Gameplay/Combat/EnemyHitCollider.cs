
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class EnemyHitCollider : RaycastHitCollider
{
    private NetworkBehaviour owner;
    
    public void InitOwner(NetworkBehaviour ownerEnemy)
    {
        owner = ownerEnemy;
    }
    
    public override void CheckHits()
    {
        if (owner == null || !owner.IsServerInitialized)
            return;
            
        base.CheckHits();
    }
    
    public List<Collider> GetHitTargets()
    {
        return new List<Collider>(hitTargets);
    }
}
