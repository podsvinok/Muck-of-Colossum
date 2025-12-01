using System;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.SceneManagement
{
    public interface ISceneLoader
    {
        public UniTask LoadSceneAsync(string sceneName, Action onLoaded = null);
        public void LoadSceneNetwork(string sceneName);
    }
}