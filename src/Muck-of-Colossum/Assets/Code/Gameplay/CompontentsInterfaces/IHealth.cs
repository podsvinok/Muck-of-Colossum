using UnityEngine;
using System;

public interface IHealth 
{
    float CurrentHealth { get; }
    float MaxHealth { get; }
    bool IsDead { get; }
    event Action OnDeath;
    event Action<float> OnDamaged;
    void TakeDamage(float damage, Vector3 hitPoint = default);
}
