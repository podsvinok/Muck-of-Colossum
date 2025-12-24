using System.Collections.ObjectModel;
using Code.Gameplay.Item;
using Code.Gameplay.Item.Factory;
using Cysharp.Threading.Tasks;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Code.Gameplay.Inventory
{
    public class Inventory : NetworkBehaviour
    {
        [SerializeField] private Transform itemSpawnPoint;
        protected ObservableCollection<InventoryItem> inventoryItems = new();
        protected IItemFactory itemFactory;

        [Inject]
        public void Construct(IItemFactory itemFactory)
        {
            this.itemFactory = itemFactory;
        }
        
        public virtual bool TryAddItem(ItemPreset preset, int quantity)
        {
            if (TryStack(preset, quantity))
                return true;

            return TryAddNewItem(preset, quantity);
        }

        private bool TryStack(ItemPreset preset, int quantity)
        {
            for (var i = 0; i < inventoryItems.Count; i++)
            {
                var item = inventoryItems[i];
                if (item.preset != preset)
                    continue;

                item.quantity += quantity;
                inventoryItems[i] = item;
                return true;
            }

            return false;
        }

        private bool TryAddNewItem(ItemPreset preset, int quantity)
        {
            for (var i = 0; i < inventoryItems.Count; i++)
            {
                var item = inventoryItems[i];
                if (item.preset)
                    continue;

                item.preset = preset;
                item.quantity = quantity;
                inventoryItems[i] = item;
                return true;
            }
            return false;
        }

        protected virtual void RemoveItem(int index, int amount)
        {
            var item = inventoryItems[index];

            item.quantity -= amount;
            if (item.quantity <= 0)
                item.preset = null;

            inventoryItems[index] = item;
        }

        public virtual void DropItem(int index, int amount)
        {
            var item = inventoryItems[index];
            if (item.preset == null)
                return;

            amount = Mathf.Min(item.quantity, amount);
            for (int i = 0; i < amount; i++)
                SpawnItem(item.preset);
            
            RemoveItem(index, amount);
        }

        public virtual void Interact(int index)
        {
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnItem(ItemPreset item) => 
            itemFactory.SpawnItem(item.prefab, itemSpawnPoint.position);
    }
}