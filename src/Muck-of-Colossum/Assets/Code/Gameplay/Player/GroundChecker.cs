
using FishNet.Object;
using UnityEngine;

public class GroundChecker : NetworkBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField, Range(0.01f, 1f)] private float groundCheckRadius;

    public bool IsTouches { get; private set; }
    
    private void Update()
    {
        if (IsOwner == false)
            return;
        
        IsTouches = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);
    }
}
