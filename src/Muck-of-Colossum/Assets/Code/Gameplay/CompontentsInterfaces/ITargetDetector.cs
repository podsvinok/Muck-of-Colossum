using UnityEngine;

public interface ITargetDetector 
{
    Transform CurrentTarget { get; }
    bool HasTarget { get; }
    void DetectTargets();
}
