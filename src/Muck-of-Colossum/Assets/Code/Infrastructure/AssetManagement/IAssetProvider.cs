using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
    public interface IAssetProvider
    {
        UniTask<GameObject> LoadAsset(string path);
        UniTask<T> LoadAsset<T>(string path) where T : Component;
    }
}