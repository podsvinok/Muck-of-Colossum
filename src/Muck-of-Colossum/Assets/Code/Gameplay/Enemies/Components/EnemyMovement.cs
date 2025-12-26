using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour, IEnemyMovement
{
    private NavMeshAgent agent;
    private float moveSpeed;
    
    public bool IsMoving { get; private set; }
    
    public void Initialize(NavMeshAgent agent, float speed)
    {
        this.agent = agent;
        this.moveSpeed = speed;
    }
    
    public void MoveTo(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        agent.destination = targetPosition;
        IsMoving = true;
    }
    
    public void Stop()
    {
        IsMoving = false;
    }
}
