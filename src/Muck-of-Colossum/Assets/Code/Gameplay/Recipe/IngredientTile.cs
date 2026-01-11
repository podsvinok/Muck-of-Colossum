using System;
using Code.Gameplay.InventorySystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Recipe
{
    public class IngredientTile : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text quantityText;
        private Color bgColor;
        private int index;

        private void Awake()
        {
            bgColor = background.color;
            ResetTile();
        }

        public void SetItem(InventoryItem item)
        {
            icon.color = Color.white;
            icon.sprite = item.preset.icon;
            background.color = bgColor;
            quantityText.text = item.quantity.ToString();
        }

        public void ResetTile()
        {
            background.color = Color.clear;
            icon.color = Color.clear;
            icon.sprite = null;
            quantityText.text = "";
        }
    }
}