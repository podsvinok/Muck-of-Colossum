using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
    public interface IAssetProvider
    {
        UniTask<GameObject> LoadAsync(string path);
        UniTask<T> LoadAsync<T>(string path) where T : Object;
        T[] LoadAll<T>(string path) where T : Object;
    }
}