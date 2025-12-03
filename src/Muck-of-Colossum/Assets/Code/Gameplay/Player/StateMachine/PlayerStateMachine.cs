using System.Collections.Generic;
using System.Linq;

public class PlayerStateMachine : IStateSwitcher
{
    private List<IPlayerState> states;
    private IPlayerState _currentPlayerState;

    public PlayerStateMachine(Player player)
    {
        PlayerStateMachineData data = new PlayerStateMachineData();
        data.CameraController = player.PlayerCamera;
        
        states = new List<IPlayerState>()
        {
            new GroundIdlePlayerState(this, data, player),
            new RunningPlayerState(this, data, player),
            new JumpingPlayerState(this, data, player),
            new FallingPlayerState(this, data, player),
            new ClimbPlayerIdleState(this, data, player),
            new ClimbPlayerMoveState(this, data, player),
        };
        
        
        _currentPlayerState = states[0];
        _currentPlayerState.Enter();
    }

    public void SwitchState<T>() where T : IPlayerState
    {
        IPlayerState playerState = states.FirstOrDefault(state => state is T);
        
        _currentPlayerState.Exit();
        _currentPlayerState = playerState;
        _currentPlayerState.Enter();
    }
    
    public void HandleInput() => _currentPlayerState.HandleInput();
    
    public void Update() => _currentPlayerState.Update();
}
