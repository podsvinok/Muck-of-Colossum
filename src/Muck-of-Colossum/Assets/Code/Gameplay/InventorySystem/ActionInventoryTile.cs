using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.InventorySystem
{
    public class ActionInventoryTile : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image selectedFrame;
        [SerializeField] private TMP_Text quantityText;
        
        private ActionInventoryView inventoryView;
        private int index;

        public void Initialize(ActionInventoryView inventoryView, int index)
        {
            this.inventoryView = inventoryView;
            this.index = index;
        }

        public void ToggleActive(bool toggle) => 
            selectedFrame.color = toggle ? Color.white : Color.clear;

        public void SetItem(InventoryItem item)
        {
            if (!item.preset)
            {
                ResetTile();
                return;
            }

            icon.color =  Color.white;
            icon.sprite = item.preset.icon;
            quantityText.text = item.quantity.ToString();
        }
        
        private void ResetTile()
        {
            icon.color = Color.clear;
            icon.sprite = null;
            quantityText.text = "";
        }
    }
}