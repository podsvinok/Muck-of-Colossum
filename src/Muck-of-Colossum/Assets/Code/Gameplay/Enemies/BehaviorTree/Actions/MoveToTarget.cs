using Unity.Behavior;
using UnityEngine;

public class MoveToTarget : Action
{
    [SerializeField] private Blackboard blackboard;
    [SerializeField] private float stoppingDistance = 1.5f;

    protected override Status OnStart()
    {
        return base.OnStart();
        
        
    }

    // protected override Status OnUpdate()
    // {
    //     if (blackboard.currentTarget == null)
    //         return Status.Failure;
    //     
    //     float distance = Vector3.Distance(blackboard.enemy.transform.position, blackboard.currentTarget.position);
    //     
    //     if (distance <= stoppingDistance)
    //     {
    //         blackboard.enemy.Movement.Stop();
    //         return Status.Success;
    //     }
    //     
    //     blackboard.enemy.Movement.MoveTo(blackboard.currentTarget.position);
    //     return Status.Running;
    // }
}
