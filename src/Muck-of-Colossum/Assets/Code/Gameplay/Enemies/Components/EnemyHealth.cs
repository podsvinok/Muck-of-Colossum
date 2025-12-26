using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using System;
using FishNet.CodeGenerating;

public class EnemyHealth : NetworkBehaviour, IHealth
{
    [AllowMutableSyncType]
    private readonly SyncVar<float> currentHealth = new SyncVar<float>(0f);
    private float maxHealth;
    
    public float CurrentHealth => currentHealth.Value;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentHealth.Value <= 0;
    
    public event Action OnDeath;
    public event Action<float> OnDamaged;
    
    public void Initialize(float health)
    {
        maxHealth = health;
        currentHealth.Value = health;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamage(float damage, Vector3 hitPoint = default)
    {
        if (IsDead) return;
        
        currentHealth.Value -= damage;
        OnDamaged?.Invoke(damage);
        
        if (IsDead)
        {
            OnDeath?.Invoke();
        }
    }
}
