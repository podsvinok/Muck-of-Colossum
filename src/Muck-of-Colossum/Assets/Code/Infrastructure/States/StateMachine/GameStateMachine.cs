using System.Threading;
using Code.Infrastructure.States.Factory;
using Code.Infrastructure.States.GameStates;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.States.StateMachine
{
    public class GameStateMachine : IGameStateMachine
    {
        private readonly IStateFactory stateFactory;
        
        private IExitableState activeState;

        public GameStateMachine(IStateFactory stateFactory)
        {
            this.stateFactory = stateFactory;
        }

        public async UniTask Enter<TState>() where TState : class, IState
        {
            await RequestEnter<TState>();
        }

        public async UniTask Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
        {
            await RequestEnter<TState, TPayload>(payload);
        }

        private async UniTask<TState> RequestEnter<TState>() where TState : class, IState
        {
            var state = await RequestChangeState<TState>();
            await EnterState(state);
            return state;
        }

        private async UniTask<TState> RequestEnter<TState, TPayload>(TPayload payload)
            where TState : class, IPayloadState<TPayload>
        {
            var state = await RequestChangeState<TState>();
            await EnterPayloadState(state, payload);
            return state;
        }

        private async UniTask<TState> EnterState<TState>(TState state) where TState : class, IState
        {
            activeState = state;
            await state.Enter();
            return state;
        }

        private async UniTask<TState> EnterPayloadState<TState, TPayload>(TState state, TPayload payload)
            where TState : class, IPayloadState<TPayload>
        {
            activeState = state;
            await state.Enter(payload);
            return state;
        }

        private async UniTask<TState> RequestChangeState<TState>() where TState : class, IExitableState
        {
            if (activeState != null) 
                await activeState.Exit();

            return ChangeState<TState>();
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            return stateFactory.GetState<TState>();
        }
    }
}