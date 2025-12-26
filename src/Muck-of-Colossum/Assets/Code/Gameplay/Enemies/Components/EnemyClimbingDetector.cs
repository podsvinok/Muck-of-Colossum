using UnityEngine;
using System;

public class EnemyClimbingDetector : MonoBehaviour, IEnemyClimbingDetector
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float detectionRadius = 2f;
    
    public bool HasPlayerOnBack { get; private set; }
    public Transform ClimbingPlayer { get; private set; }
    
    public event Action<Transform> OnPlayerStartClimbing;
    public event Action<Transform> OnPlayerStopClimbing;
    
    private void CheckForClimbingPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, detectionRadius, playerMask);
        
        if (colliders.Length > 0 && !HasPlayerOnBack)
        {
            HasPlayerOnBack = true;
            ClimbingPlayer = colliders[0].transform;
            OnPlayerStartClimbing?.Invoke(ClimbingPlayer);
        }
        else if (colliders.Length == 0 && HasPlayerOnBack)
        {
            HasPlayerOnBack = false;
            var prevPlayer = ClimbingPlayer;
            ClimbingPlayer = null;
            OnPlayerStopClimbing?.Invoke(prevPlayer);
        }
    }
}