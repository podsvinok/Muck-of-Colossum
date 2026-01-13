using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Ritual Grounds/Configs/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [SerializeField] private RunningStateConfig runningStateConfig;
    [SerializeField] private AirborneStateConfig airborneStateConfig;
    [SerializeField] private ClimbStateConfig climbStateConfig;
    [SerializeField] private HealthConfig healthConfig;
    public RunningStateConfig RunningStateConfig => runningStateConfig;
    public AirborneStateConfig AirborneStateConfig => airborneStateConfig;
    public ClimbStateConfig ClimbStateConfig => climbStateConfig;
    public HealthConfig HealthConfig => healthConfig;
}
