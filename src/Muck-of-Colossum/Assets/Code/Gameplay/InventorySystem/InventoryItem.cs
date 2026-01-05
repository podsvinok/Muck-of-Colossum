using Code.Gameplay.Items;

namespace Code.Gameplay.InventorySystem
{
    [System.Serializable]
    public struct InventoryItem
    {
        public ItemPreset preset;
        public int quantity;
    }
}