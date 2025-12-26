using UnityEngine;


public class PlayerTargetDetector : MonoBehaviour, ITargetDetector

{
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask playerMask;

    public Transform CurrentTarget { get; private set; }
    public bool HasTarget => CurrentTarget != null;

    public void DetectTargets()
    {
        var colliders = Physics.OverlapSphere(transform.position, detectionRadius, playerMask);
        CurrentTarget = colliders.Length > 0 ? colliders[0].transform : null;
    }
}
