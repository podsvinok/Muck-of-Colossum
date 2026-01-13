using System;
using UnityEngine;

[Serializable]
public class HealthConfig
{
    [SerializeField, Range(0, 99999)] private float maxHealth;
    
    public float MaxHealth => maxHealth;
    
}