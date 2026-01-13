using UnityEngine;
using System;

public interface IHealth 
{
    float CurrentHealth
    {
        get;
        set;
    }

    float MaxHealth { get; set; }
    bool IsDead { get; }
    event Action OnDeath;
    event Action<float> OnDamaged;


    void Init(float health);
    
    void TakeDamage(float damage, Vector3 hitPoint = default);
}
