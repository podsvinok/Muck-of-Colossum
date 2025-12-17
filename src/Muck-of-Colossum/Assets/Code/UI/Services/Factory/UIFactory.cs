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

        public WindowBase CreateInventoryActiveSlots()
        {
            var windowConfig = staticData.ForWindow(WindowId.InventoryActiveSlots);
            var inventoryActiveSlots = Object.Instantiate(windowConfig.Prefab, uiRoot);
            return inventoryActiveSlots;
        }
        
        public WindowBase CreateInventory()
        {
            var windowConfig = staticData.ForWindow(WindowId.Inventory);
            var inventory = Object.Instantiate(windowConfig.Prefab, uiRoot);
            return inventory;
        }

        public async UniTask CreateUIRoot()
        {
            var instantiate = await assetProvider.LoadAsync(AssetPath.UIRoot);
            uiRoot = Object.Instantiate(instantiate).transform;
        }
    }
}