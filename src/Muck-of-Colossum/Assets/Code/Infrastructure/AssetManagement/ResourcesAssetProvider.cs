using UnityEngine;

namespace Code.Infrastructure.AssetManagement
{
    public class ResourcesAssetProvider : IAssetProvider
    {
        public GameObject LoadAsset(string path) => 
            Resources.Load<GameObject>(path);

        public T LoadAsset<T>(string path) where T : Component => 
            Resources.Load<T>(path);
    }
}