using Code.Gameplay.Cameras;
using Code.Gameplay.Player.StateMachine;
using Code.Gameplay.Player.StateMachine.States.ClimbMovement;
using Code.Infrastructure.Inputs;
using FishNet.Object;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private PlayerConfig config;
        [SerializeField] private PlayerView view;
        [SerializeField] private CameraController playerCamera;
        [SerializeField] private GroundChecker groundChecker;
        [SerializeField] private ClimbChecker climbChecker;
        [SerializeField] private CharacterController characterController;
    
        public PlayerInput Input => input;
        public CharacterController Controller => characterController;
        public CameraController PlayerCamera => playerCamera;
        public PlayerConfig Config => config;
    
        public PlayerView View => view;
    
        public GroundChecker GroundChecker => groundChecker;
        public ClimbChecker ClimbChecker => climbChecker;

        private IInputService inputService;
        private PlayerInput input;
        private PlayerStateMachine stateMachine;

        [Inject]
        public void Construct(IInputService inputService)
        {
            this.inputService = inputService;
        }

        public override void OnStartClient()
        {
            if (!IsOwner) return;
        
            input = inputService.Input;
            stateMachine = new PlayerStateMachine(this);
        }

        private void Update()
        {
            if (!IsOwner) return;
        
            stateMachine.HandleInput();
            stateMachine.Update();
        }
    }
}
