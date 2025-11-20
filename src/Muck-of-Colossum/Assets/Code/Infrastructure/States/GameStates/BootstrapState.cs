using Code.Infrastructure.States.StateMachine;
using Code.UI.LoadingCurtain;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public class BootstrapState : IState
    {
        private readonly IGameStateMachine gameStateMachine;
        private readonly LoadingCurtainProxy loadingCurtain;

        public BootstrapState(IGameStateMachine gameStateMachine,
            LoadingCurtainProxy loadingCurtain)
        {
            this.gameStateMachine = gameStateMachine;
            this.loadingCurtain = loadingCurtain;
        }

        public async UniTask Enter()
        {
            await loadingCurtain.InitializeAsync();
            await gameStateMachine.Enter<GameLoadingState>();
        }

        public UniTask Exit() => 
            default;
    }
}