using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Code.Infrastructure.SceneManagement;
using Code.Utils;
using Cysharp.Threading.Tasks;
using FishNet.Managing;
using UnityEngine;

namespace Code.Infrastructure.States.GameStates
{
    public class GameLoadingState : IState
    {
        private readonly ILevelDataProvider levelData;
        private readonly ISceneLoader sceneLoader;
        private readonly ILoadingCurtain loadingCurtain;

        public GameLoadingState(ILevelDataProvider levelData, ISceneLoader sceneLoader, ILoadingCurtain loadingCurtain)
        {
            this.sceneLoader = sceneLoader;
            this.loadingCurtain = loadingCurtain;
            this.levelData = levelData;
        }

        public async UniTask Enter()
        {
            loadingCurtain.Show();

            await UniTask.WaitForSeconds(1.5f);
            await sceneLoader.LoadScene(AssetPath.LoadingScene);
            
            loadingCurtain.Hide();
        }

        public UniTask Exit() => 
            default;
    }
}