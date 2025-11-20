using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.StaticData;
using Code.UI.LoadingCurtain;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.States.GameStates
{
    public class GameLoadingState : IState
    {
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IGameStateMachine stateMachine;

        public GameLoadingState(
            ILoadingCurtain loadingCurtain,
            IGameStateMachine stateMachine)
        {
            this.loadingCurtain = loadingCurtain;
            this.stateMachine = stateMachine;
        }

        public async UniTask Enter()
        {
            loadingCurtain.Show();
            
            await stateMachine.Enter<MainMenuLoadingState>();
        }

        public UniTask Exit() => 
            default;
    }
}