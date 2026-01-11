using Code.Gameplay.Recipe;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.StaticData;
using Code.UI.Services.Windows;
using Code.UI.Windows;
using Code.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.UI.Services.Factory
{
    public class UIFactory : IUIFactory
    {
        private readonly IStaticDataService staticData;
        private readonly IAssetProvider assetProvider;
        
        private Transform uiRoot;

        public UIFactory(
            IStaticDataService staticData,
            IAssetProvider assetProvider)
        {
            this.staticData = staticData;
            this.assetProvider = assetProvider;
        }

        public WindowBase CreateWindow(WindowId windowId)
        {
            var windowConfig = staticData.ForWindow(windowId);
            var window = Object.Instantiate(windowConfig.Prefab, uiRoot);
            return window;
        }

        public async UniTask CreateUIRoot()
        {
            var instantiate = await assetProvider.LoadAsync(AssetPath.UIRoot);
            uiRoot = Object.Instantiate(instantiate).transform;
        }

        public async UniTask<RecipeTile> CreateRecipeTile(Transform transform)
        {
            var tilePrefab = await assetProvider.LoadAsync<RecipeTile>(AssetPath.RecipeTile);
            var recipeTile = Object.Instantiate(tilePrefab, transform);
            return recipeTile;
        }
    }
}