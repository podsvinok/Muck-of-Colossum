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
        [SerializeField] private Image frame;
        [SerializeField] private TMP_Text quantityText;
        private int index;
        
        public void SetItem(InventoryItem item)
        {
            frame.color = Color.white;
            icon.color = Color.white;
            background.color = Color.white;
            icon.sprite = item.preset.icon;
            quantityText.text = item.quantity.ToString();
        }

        public void ResetTile()
        {
            frame.color = Color.clear;
            background.color = Color.clear;
            icon.color = Color.clear;
            icon.sprite = null;
            quantityText.text = "";
        }
    }
}