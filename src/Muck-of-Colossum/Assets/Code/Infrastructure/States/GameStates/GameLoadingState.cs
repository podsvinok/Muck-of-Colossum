using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public class GameLoadingState : IState
    {
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IGameStateMachine stateMachine;

        public GameLoadingState(IGameStateMachine stateMachine, ILoadingCurtain loadingCurtain)
        {
            this.loadingCurtain = loadingCurtain;
            this.stateMachine = stateMachine;
        }

        public async UniTask Enter()
        {
            loadingCurtain.Show();

            await stateMachine.Enter<MainMenuLoadingState>();
            
            loadingCurtain.Hide();
        }

        public UniTask Exit() => 
            default;
    }
}