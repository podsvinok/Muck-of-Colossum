using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.InventorySystem
{
    public class InventoryCursor : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image itemNameImage;
        [SerializeField] private TMP_Text quantityText;
        [SerializeField] private TMP_Text nameText;
        
        private void Update() => 
            transform.position = Input.mousePosition;

        public void SetHoldingItem(InventoryItem item)
        {
            if (item.preset == null)
                return;

            ClearItemNameVisual();
            SetHoldingItemVisual(item);
        }

        public void SetItemName(InventoryItem item)
        {
            if (item.preset == null)
            {
                ClearItemNameVisual();
                return;
            }
            SetItemNameVisual(item);
        }

        public void Hide()
        {
            ClearHoldingItemVisual();
            ClearItemNameVisual();
        }

        private void SetHoldingItemVisual(InventoryItem item)
        {
            icon.color = Color.white;
            icon.sprite = item.preset.icon;
            quantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";
        }

        private void ClearHoldingItemVisual()
        {
            icon.color = Color.clear;
            quantityText.text = "";
        }

        private void SetItemNameVisual(InventoryItem item)
        {
            itemNameImage.color = Color.white;
            nameText.text = item.preset.itemName;
        }

        public void ClearItemNameVisual()
        {
            itemNameImage.color = Color.clear;
            nameText.text = "";
        }
    }
}