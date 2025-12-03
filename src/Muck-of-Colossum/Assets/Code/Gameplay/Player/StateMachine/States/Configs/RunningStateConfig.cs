using System;
using UnityEngine;

[Serializable]
public class RunningStateConfig
{
    [SerializeField, Range(0, 100)] private float speed;
    [SerializeField, Range(0, 100)] private float rotationSpeed;
    
    public float Speed => speed;
    public float RotationSpeed => rotationSpeed;
}
