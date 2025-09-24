using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
    public class ResourcesAssetProvider : IAssetProvider
    {
        public async UniTask<GameObject> LoadAsset(string path)
        {
            var request = Resources.LoadAsync<GameObject>(path);

            await request;

            var asset = request.asset as GameObject;
            return asset;
        }

        public async UniTask<T> LoadAsset<T>(string path) where T : Component
        {
            var request = Resources.LoadAsync<T>(path);

            await request;

            var asset = request.asset as T;
            return asset;
        }
    }
}