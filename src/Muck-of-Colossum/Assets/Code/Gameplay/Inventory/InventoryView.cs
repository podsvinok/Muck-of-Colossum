using UnityEngine;

namespace Code.Gameplay.Inventory
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private InventoryTile[] inventoryTiles;

        private Inventory inventory;

        public void Initialize(Inventory inventory)
        {
            this.inventory = inventory;
        }

        private void Awake()
        {
            for (int i = 0; i < inventoryTiles.Length; i++) 
                inventoryTiles[i].Initialize(this, i);
        }

        public void RedrawEverything(InventoryItem[] inventoryItems)
        {
            for (int i = 0; i < inventoryItems.Length; i++)
            {
                var item = inventoryItems[i];
                if (inventoryItems.Length <= i)
                {
                    Debug.LogError($"More items than tiles. {i} > {inventoryItems.Length}", this);
                    return;
                }
                inventoryTiles[i].SetItem(item);
            }
        }

        public void DropItem(int index) => 
            inventory.DropItem(index, 1);

        public void Interact(int index) => 
            inventory.Interact(index);

        public int GetInventoryTilesLength() => 
            inventoryTiles.Length;
    }
}