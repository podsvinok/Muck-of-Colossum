using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Zenject;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.SceneManagement
{
    public class SceneLoader : ISceneLoader
    {
        public async UniTask LoadScene(string name, Action onLoaded = null)
        {
            if (SceneManager.GetActiveScene().name == name)
            {
                onLoaded?.Invoke();
                return;
            }

            var waitNextScene = SceneManager.LoadSceneAsync(name);

            while (!waitNextScene.isDone)
                await UniTask.Yield();

            onLoaded?.Invoke();
        }
    }
}