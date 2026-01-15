using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.StaticData;
using Code.UI.LoadingCurtain;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public class GameLoadingState : IState
    {
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IGameStateMachine stateMachine;
        private readonly IStaticDataService staticData;

        public GameLoadingState(
            ILoadingCurtain loadingCurtain,
            IGameStateMachine stateMachine, 
            IStaticDataService staticData)
        {
            this.loadingCurtain = loadingCurtain;
            this.stateMachine = stateMachine;
            this.staticData = staticData;
        }

        public async UniTask Enter()
        {
            await staticData.LoadAllAsync();
            await stateMachine.Enter<MainMenuLoadingState>();
        }

        public UniTask Exit() => 
            default;
    }
}