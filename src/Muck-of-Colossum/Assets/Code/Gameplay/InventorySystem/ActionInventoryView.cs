using UnityEngine;

namespace Code.Gameplay.InventorySystem
{
    public class ActionInventoryView : MonoBehaviour
    {
        [SerializeField] private ActionInventoryTile[] inventoryTiles;
        private ActionInventoryTile currentActiveSlot;

        public void Initialize()
        {
            for (int i = 0; i < inventoryTiles.Length; i++) 
                inventoryTiles[i].Initialize(this, i);
        }
        
        public void RedrawEverything(InventoryItem[] inventoryItems)
        {
            for (int i = 0; i < inventoryTiles.Length; i++)
            {
                var item = inventoryItems[i];
                inventoryTiles[i].SetItem(item);
            }
        }

        public void SetActiveSlot(int index)
        {
            if (currentActiveSlot) 
                currentActiveSlot.ToggleActive(false);
            
            inventoryTiles[index].ToggleActive(true);
            currentActiveSlot = inventoryTiles[index];
        }

        public int GetActiveSlotsCount() => 
            inventoryTiles.Length;
    }
}