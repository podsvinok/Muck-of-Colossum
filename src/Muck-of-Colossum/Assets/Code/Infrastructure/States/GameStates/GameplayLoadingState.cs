using Code.Infrastructure.SceneManagement;
using Code.Infrastructure.States.StateMachine;
using Code.Utils;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public class GameplayLoadingState: IState
    {
        private readonly ISceneLoader sceneLoader;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IGameStateMachine stateMachine;

        public GameplayLoadingState(ISceneLoader sceneLoader, ILoadingCurtain loadingCurtain, IGameStateMachine stateMachine)
        {
            this.sceneLoader = sceneLoader;
            this.loadingCurtain = loadingCurtain;
            this.stateMachine = stateMachine;
        }

        public async UniTask Enter()
        {
            loadingCurtain.Show();
            
            await UniTask.WaitForSeconds(2);
            await sceneLoader.LoadScene(AssetPath.LoadingScene);
            await sceneLoader.LoadScene(AssetPath.GameScene);
            
            await stateMachine.Enter<GameplayLoopState>();
            
            loadingCurtain.Hide();
        }

        public UniTask Exit() => 
            default;
    }
}