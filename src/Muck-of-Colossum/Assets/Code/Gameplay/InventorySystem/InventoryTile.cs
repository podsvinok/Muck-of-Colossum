using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Gameplay.InventorySystem
{
    public class InventoryTile : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text quantityText;

        private InventoryView inventoryView;
        private int index;

        public void Initialize(InventoryView inventoryView, int index)
        {
            this.inventoryView = inventoryView;
            this.index = index;
        }

        public void SetItem(InventoryItem item)
        {
            if (!item.preset)
            {
                ResetTile();
                return;
            }

            icon.sprite = item.preset.icon;
            quantityText.text = item.quantity.ToString();
            icon.color = Color.white;
        }
        
        private void ResetTile()
        {
            icon.color = Color.clear;
            icon.sprite = null;
            quantityText.text = "";
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    inventoryView.Interact(index);
                    break;
                case PointerEventData.InputButton.Right:
                    inventoryView.DropItem(index);
                    break;
            }
        }
    }
}