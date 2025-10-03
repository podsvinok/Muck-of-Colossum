using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
    public interface IAssetProvider
    {
        UniTask<GameObject> Load(string path);
        UniTask<T> Load<T>(string path) where T : Component;
    }
}