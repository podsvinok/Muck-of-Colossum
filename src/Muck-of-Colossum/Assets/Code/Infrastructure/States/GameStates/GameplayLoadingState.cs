using Code.Infrastructure.SceneManagement;
using Code.Infrastructure.States.StateMachine;
using Code.Network;
using Code.Utils;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public class GameplayLoadingState: IState
    {
        private readonly ISceneLoader sceneLoader;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IGameStateMachine stateMachine;
        private readonly NetworkSceneLoader network;

        public GameplayLoadingState(
            ISceneLoader sceneLoader,
            ILoadingCurtain loadingCurtain,
            IGameStateMachine stateMachine,
            NetworkSceneLoader network)
        {
            this.sceneLoader = sceneLoader;
            this.loadingCurtain = loadingCurtain;
            this.stateMachine = stateMachine;
            this.network = network;
        }

        public async UniTask Enter()
        {
            loadingCurtain.Show();
            
            await sceneLoader.LoadScene(AssetPath.LoadingScene);
            await sceneLoader.LoadScene(AssetPath.GameScene);
            
            await stateMachine.Enter<GameplayLoopState>();
            
            loadingCurtain.Hide();
        }

        public UniTask Exit() => 
            default;
    }
}