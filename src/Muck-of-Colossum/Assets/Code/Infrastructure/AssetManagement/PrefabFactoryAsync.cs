using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.AssetManagement
{
    public class PrefabFactoryAsync<TComponent> : IFactory<string, UniTask<TComponent>>
    {
        private readonly IInstantiator instantiator;
        private readonly IAssetProvider assetProvider;
        
        public PrefabFactoryAsync(IInstantiator instantiator, IAssetProvider assetProvider)
        {
            this.instantiator = instantiator;
            this.assetProvider = assetProvider;
        }

        public async UniTask<TComponent> Create(string path)
        {
            GameObject prefab = await assetProvider.Load(path);
            GameObject newObject = instantiator.InstantiatePrefab(prefab);
            
            return newObject.GetComponent<TComponent>();
        }
    }
}