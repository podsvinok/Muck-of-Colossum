using Code.Infrastructure.States.GameStates;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.StateMachine
{
    public interface IGameStateMachine
    {
        UniTaskVoid Enter<TState>() where TState : class, IState;
        UniTaskVoid Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>;
    }
}