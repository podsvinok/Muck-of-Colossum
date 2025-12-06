using System;
using UnityEngine;

[Serializable]
public class RunningStateConfig
{
    [SerializeField, Range(0, 100)] private float speed;
    [SerializeField, Range(0, 100)] private float rotationSpeed;
    [SerializeField, Range(-50, 0)] private float gravityForceOnGround;
    
    public float Speed => speed;
    public float RotationSpeed => rotationSpeed;
    public float GravityForceOnGround => gravityForceOnGround;
}
