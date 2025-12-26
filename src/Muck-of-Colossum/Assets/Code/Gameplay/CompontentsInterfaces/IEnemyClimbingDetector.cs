using UnityEngine;
using System;

public interface IEnemyClimbingDetector
{
    bool HasPlayerOnBack { get; }
    Transform ClimbingPlayer { get; }
    event Action<Transform> OnPlayerStartClimbing;
    event Action<Transform> OnPlayerStopClimbing;
}
