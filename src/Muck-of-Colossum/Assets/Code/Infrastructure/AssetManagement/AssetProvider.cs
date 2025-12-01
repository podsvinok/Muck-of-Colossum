using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        public async UniTask<GameObject> LoadAsync(string path)
        {
            var request = Resources.LoadAsync<GameObject>(path);
            await request;
            
            if (request.asset == null)
                Debug.LogError($"Attempt to load {path} was unsuccessful");
            
            return request.asset as GameObject;
        }

        public async UniTask<T> LoadAsync<T>(string path) where T : Object
        {
            var request = Resources.LoadAsync<T>(path);
            await request;
            
            if (request.asset == null)
                Debug.LogError($"Attempt to load {path} was unsuccessful");
            
            return request.asset as T;
        }

        public T[] LoadAll<T>(string path) where T : Object
        {
            var obj = Resources.LoadAll<T>(path) as T[];
            
            if (obj == null)
                Debug.LogError($"Attempt to load {path} was unsuccessful");
            
            return obj;
        }
    }
}