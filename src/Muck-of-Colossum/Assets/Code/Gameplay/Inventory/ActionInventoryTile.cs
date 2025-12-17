using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Gameplay.Inventory
{
    public class ActionInventoryTile : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TMP_Text quantityText;
        [SerializeField] private Color activeColor;
        
        private ActionInventoryView inventoryView;
        private Color originalColor;
        private int index;

        private void Awake() => 
            originalColor = backgroundImage.color;

        public void Initialize(ActionInventoryView inventoryView, int index)
        {
            this.inventoryView = inventoryView;
            this.index = index;
        }

        public void ToggleActive(bool toggle) => 
            backgroundImage.color = toggle ? activeColor : originalColor;

        public void SetItem(InventoryItem item)
        {
            if (!item.preset)
            {
                ResetTile();
                return;
            }

            icon.sprite = item.preset.icon;
            quantityText.text = item.quantity.ToString();
            icon.color =  Color.white;
        }
        
        private void ResetTile()
        {
            icon.color = Color.clear;
            icon.sprite = null;
            quantityText.text = "";
        }
    }
}