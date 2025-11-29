
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField, Range(0.01f, 1f)] private float groundCheckRadius;

    public bool IsTouches { get; private set; }
    
    private void Update()
    {
        IsTouches = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);
    }
}
