
using Code.Gameplay.Player.StateMachine.States;

namespace Code.Gameplay.Player.StateMachine
{
    public  interface IStateSwitcher 
    {
        void SwitchState<T>() where T : IPlayerState;
    }
}
