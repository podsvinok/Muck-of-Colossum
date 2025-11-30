using FishNet.Object;
using LiteNetLib;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : NetworkBehaviour
{
    [SerializeField] private PlayerConfig config;
    [SerializeField] private PlayerView view;
    [SerializeField] private CameraController playerCamera;
    [SerializeField] private GroundChecker groundChecker;
    [SerializeField] private ClimbChecker climbChecker;
    
    private PlayerInput input;
    private PlayerStateMachine stateMachine;
    private CharacterController characterController;
    
    public PlayerInput Input => input;
    public CharacterController Controller => characterController;
    public CameraController PlayerCamera => playerCamera;
    public PlayerConfig Config => config;
    
    public PlayerView View => view;
    
    public GroundChecker GroundChecker => groundChecker;
    public ClimbChecker ClimbChecker => climbChecker;
    

    private void Awake()
    {
        view.Init();
        playerCamera.Init();
        characterController = GetComponent<CharacterController>();
        input = new PlayerInput();
        stateMachine = new PlayerStateMachine(this);
    }

    private void Update()
    {
        if (IsOwner == false)
            return;
        
        stateMachine.HandleInput();
        stateMachine.Update();
    }
    
    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();
}
