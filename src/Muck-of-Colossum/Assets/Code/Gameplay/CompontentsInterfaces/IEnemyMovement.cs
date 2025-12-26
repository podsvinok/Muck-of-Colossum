using UnityEngine;

public interface IEnemyMovement 
{
    void MoveTo(Vector3 targetPosition);
    void Stop();
    bool IsMoving { get; }
}
