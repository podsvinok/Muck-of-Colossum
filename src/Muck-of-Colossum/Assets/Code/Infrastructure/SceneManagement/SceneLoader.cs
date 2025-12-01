using System;
using Cysharp.Threading.Tasks;
using FishNet.Managing;
using FishNet.Managing.Scened;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Code.Infrastructure.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        private readonly NetworkManager networkManager;

        public SceneLoader(NetworkManager networkManager)
        {
            this.networkManager = networkManager;
        }

        public async UniTask LoadSceneAsync(string sceneName, Action onLoaded = null)
        {
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                onLoaded?.Invoke();
                return;
            }

            var waitNextScene = SceneManager.LoadSceneAsync(sceneName);

            while (!waitNextScene.isDone)
                await UniTask.Yield();

            onLoaded?.Invoke();
        }

        public void LoadSceneNetwork(string sceneToLoad)
        {
            var sceneLoadData = new SceneLoadData(sceneToLoad)
            {
                ReplaceScenes = ReplaceOption.All
            };

            networkManager.SceneManager.LoadGlobalScenes(sceneLoadData);
        }
    }
}