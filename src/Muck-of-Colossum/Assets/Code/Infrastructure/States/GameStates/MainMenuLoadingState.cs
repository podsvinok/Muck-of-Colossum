using Code.Infrastructure.SceneManagement;
using Code.Infrastructure.States.StateMachine;
using Code.Utils;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public class MainMenuLoadingState : IState
    {
        private readonly ISceneLoader sceneLoader;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IGameStateMachine stateMachine;

        public MainMenuLoadingState(
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
            loadingCurtain.Show();
            
            await sceneLoader.LoadScene(AssetPath.LoadingScene);
            await sceneLoader.LoadScene(AssetPath.MainMenuScene);
            
            await stateMachine.Enter<MainMenuState>();
            
            loadingCurtain.Hide();
        }

        public UniTask Exit() => 
            default;
    }
}