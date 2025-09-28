using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        public async UniTask<GameObject> LoadAsset(string path)
        {
            var request = Resources.LoadAsync<GameObject>(path);
            await request;
            
            return request.asset as GameObject;
        }

        public async UniTask<T> LoadAsset<T>(string path) where T : Component
        {
            var request = Resources.LoadAsync<T>(path);
            await request;
            
            return request.asset as T;
        }
    }
}