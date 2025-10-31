using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.SceneManagement
{
    public interface ISceneLoader
    {
        UniTask LoadScene(string sceneName, Action onLoaded = null);
        void LoadSceneNetwork(string sceneName);
    }
}