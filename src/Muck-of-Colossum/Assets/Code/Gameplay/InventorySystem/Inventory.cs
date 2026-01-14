using System.Collections.ObjectModel;
using Code.Gameplay.Items;
using Code.Gameplay.Items.Factory;
using FishNet.Object;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.InventorySystem
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

        public virtual void DropItem(int index, int amount)
        {
            var item = inventoryItems[index];
            if (item.preset == null)
                return;

            amount = Mathf.Min(item.quantity, amount);
            for (int i = 0; i < amount; i++)
                SpawnItem(item.preset, amount);
            
            RemoveItem(index, amount);
        }

        public bool HasItems(InventoryItem ingredient)
        {
            int needed = ingredient.quantity;
            int current = 0;

            foreach (var item in inventoryItems)
            {
                if (item.preset == ingredient.preset) 
                    current += item.quantity;
            }

            return current >= needed;
        }

        public bool HasAllIngredients(InventoryItem[] ingredients)
        {
            foreach (var ingredient in ingredients)
                if (!HasItems(ingredient)) return false;
            
            return true;
        }

        public void RemoveItems(InventoryItem[] ingredients)
        {
            foreach (var ingredient in ingredients) 
                RemoveItemByPreset(ingredient.preset, ingredient.quantity);
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

        protected void RemoveItemByPreset(ItemPreset preset, int amount)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                var item = inventoryItems[i];
                if (item.preset == preset)
                    RemoveItem(i, amount);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SpawnItem(ItemPreset item, int amount) => 
            itemFactory.SpawnItem(item.prefab, itemSpawnPoint.position, amount);
    }
}