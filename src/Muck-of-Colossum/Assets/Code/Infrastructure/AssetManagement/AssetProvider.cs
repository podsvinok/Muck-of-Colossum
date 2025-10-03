using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider
    {
        public async UniTask<GameObject> Load(string path)
        {
            var request = Resources.LoadAsync<GameObject>(path);
            await request;
            
            return request.asset as GameObject;
        }

        public async UniTask<T> Load<T>(string path) where T : Component
        {
            var request = Resources.LoadAsync<T>(path);
            await request;
            
            return request.asset as T;
        }
    }
}