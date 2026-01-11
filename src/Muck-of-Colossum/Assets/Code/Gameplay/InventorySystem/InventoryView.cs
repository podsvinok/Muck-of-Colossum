using System;
using UnityEngine;

namespace Code.Gameplay.InventorySystem
{
    public class InventoryView : MonoBehaviour
    {
        public event Action<int, InventoryCursor> SlotPointerDown;
        public event Action<int, InventoryCursor> SlotPointerExit;
        public event Action<int, InventoryCursor> SlotPointerEnter;
        
        [SerializeField] private InventoryTile[] inventoryTiles;
        [SerializeField] private InventoryCursor inventoryCursor;
        
        private Inventory inventory;

        public void Initialize(Inventory inventory)
        {
            this.inventory = inventory;
            for (int i = 0; i < inventoryTiles.Length; i++) 
                inventoryTiles[i].Initialize(this, i);
        }

        public void RedrawEverything(InventoryItem[] inventoryItems)
        {
            for (int i = 0; i < inventoryItems.Length; i++)
            {
                if (i >= inventoryTiles.Length) break;
                var item = inventoryItems[i];
                inventoryTiles[i].SetItem(item);
            }
        }
        
        public void OnTileClicked(int index) => 
            SlotPointerDown?.Invoke(index, inventoryCursor);

        public void OnTilePointEnter(int index) => 
            SlotPointerEnter?.Invoke(index, inventoryCursor);

        public void OnTilePointExit(int index) => 
            SlotPointerExit?.Invoke(index, inventoryCursor);

        public void DropItem(int index) => 
            inventory.DropItem(index, 1);

        public int GetInventoryTilesLength() => 
            inventoryTiles.Length;
    }
}