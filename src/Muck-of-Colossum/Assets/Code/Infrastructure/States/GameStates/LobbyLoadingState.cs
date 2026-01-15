using Code.Infrastructure.SceneManagement;
using Code.Infrastructure.States.StateMachine;
using Code.UI.LoadingCurtain;
using Code.Utils;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public class LobbyLoadingState : IState
    {
        private readonly ISceneLoader sceneLoader;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IGameStateMachine stateMachine;

        public LobbyLoadingState(
            ISceneLoader sceneLoader,
            ILoadingCurtain loadingCurtain,
            IGameStateMachine stateMachine)
        {
            this.sceneLoader = sceneLoader;
            this.loadingCurtain = loadingCurtain;
            this.stateMachine = stateMachine;
        }

        public async UniTask Enter()
        {
            await sceneLoader.LoadSceneAsync(Scenes.LobbyScene);
            await stateMachine.Enter<LobbyState>();
        }

        public UniTask Exit() => 
            default;
    }
}