using Code.Gameplay.Player.StateMachine.States.Configs;
using UnityEngine;

namespace Code.Gameplay.Player
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "Ritual Grounds/Configs/CharacterConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [SerializeField] private RunningStateConfig runningStateConfig;
        [SerializeField] private AirborneStateConfig airborneStateConfig;
        [SerializeField] private ClimbStateConfig climbStateConfig;
        public RunningStateConfig RunningStateConfig => runningStateConfig;
        public AirborneStateConfig AirborneStateConfig => airborneStateConfig;
        public ClimbStateConfig ClimbStateConfig => climbStateConfig;
    }
}
