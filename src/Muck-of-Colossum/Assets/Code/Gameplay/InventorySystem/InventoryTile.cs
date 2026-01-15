using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Gameplay.InventorySystem
{
    public class InventoryTile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
        [SerializeField] private Image selectedFrame;
        [SerializeField] private TMP_Text quantityText;
        
        private InventoryView inventoryView;
        private int index;

        public void Initialize(InventoryView inventoryView, int index)
        {
            this.inventoryView = inventoryView;
            this.index = index;
        }
        
        private void OnEnable()
        {
            if (inventoryView)
                OnPointerExit(null);
        }

        public void SetItem(InventoryItem item)
        {
            if (!item.preset)
            {
                ResetTile();
                return;
            }

            icon.color = Color.white;
            icon.sprite = item.preset.icon;
            quantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";
        }

        private void ResetTile()
        {
            icon.color = Color.clear;
            icon.sprite = null;
            quantityText.text = "";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            selectedFrame.color = Color.white;
            inventoryView.OnTilePointEnter(index);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            selectedFrame.color = Color.clear;
            inventoryView.OnTilePointExit(index);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    inventoryView.OnTileClicked(index); 
                    break;
                case PointerEventData.InputButton.Right:
                    inventoryView.DropItem(index);
                    break;
            }
        }
    }
}