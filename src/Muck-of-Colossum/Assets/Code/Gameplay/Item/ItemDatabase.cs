using System.Collections.Generic;
using Code.Infrastructure.AssetManagement;
using Code.Utils;
using FishNet.Managing;
using UnityEngine;

namespace Code.Gameplay.Item
{
    public class ItemDatabase
    {
        private Dictionary<string, ItemPreset> itemPresets = new();
        private readonly NetworkManager networkManager;
        private readonly IAssetProvider assets;

        public ItemDatabase(
            NetworkManager networkManager,
            IAssetProvider assets)
        {
            this.networkManager = networkManager;
            this.assets = assets;
        }

        public void LoadItems()
        {
            var presets = assets.LoadAll<ItemPreset>(AssetPath.ItemPresets);
            foreach (var preset in presets)
            {
                if (!itemPresets.TryAdd(preset.uid, preset))
                    Debug.LogError($"Duplicate {preset.uid}");
            }
        }

        public bool TryGetItemPreset(string uid, out ItemPreset preset)
        {
            if (string.IsNullOrEmpty(uid))
            {
                preset = null;
                return false;
            }

            return itemPresets.TryGetValue(uid, out preset);
        }
    }
}