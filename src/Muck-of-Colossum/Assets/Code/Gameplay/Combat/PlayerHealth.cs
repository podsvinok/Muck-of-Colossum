
using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour, IHealth
{
    private readonly SyncVar<float> currentHealth = new SyncVar<float>(0f);
    private float maxHealth;
    
    public float CurrentHealth  { 
        get => currentHealth.Value;
        set => currentHealth.Value = value > MaxHealth ? MaxHealth : value;
    }
    public float MaxHealth {get => maxHealth; set => maxHealth = value; }
    public bool IsDead => currentHealth.Value <= 0;
    
    public event Action OnDeath;
    public event Action<float> OnDamaged;
    
    public void Init(float health)
    {
        if (!IsServerInitialized) return;
        
        maxHealth = health;
        currentHealth.Value = health;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamage(float damage, Vector3 hitPoint = default)
    {
        Debug.Log("I take damage");
        if (IsDead) return;
        
        currentHealth.Value -= damage;
        NotifyDamageObservers(damage);
        
        if (IsDead)
        {
            NotifyDeathObservers();
        }
    }
    
    [ObserversRpc]
    private void NotifyDamageObservers(float damage)
    {
        OnDamaged?.Invoke(damage);
    }

    [ObserversRpc]
    private void NotifyDeathObservers()
    {
        OnDeath?.Invoke();
    }
}
