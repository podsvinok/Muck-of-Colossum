using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DetectPlayer", story: "[Enemy] try detects [target]", category: "Action", id: "12f956b96946b6eb8e124cb08da591cc")]
public partial class DetectPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<Enemy> Enemy;
    [SerializeReference] public BlackboardVariable<Transform> Target;

    protected override Status OnStart()
    {
        Debug.Log(Enemy.Value == null);
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Enemy.Value.TargetDetector.DetectTargets();
        Debug.Log(Enemy.Value.TargetDetector.GetType());
        Debug.Log($"HasTarget: {Enemy.Value.TargetDetector.HasTarget}");
        Debug.Log($"CurrentTarget: {Enemy.Value.TargetDetector.CurrentTarget}");
        if (Enemy.Value.TargetDetector.HasTarget)
        {
            Target.Value = Enemy.Value.TargetDetector.CurrentTarget;
            return Status.Success;
        }
        
        return Status.Failure;
    }

    protected override void OnEnd()
    {

    }
}

