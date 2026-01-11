using System;
using Code.Gameplay.InventorySystem;
using Code.UI.Services.Factory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.Gameplay.Recipe
{
    public class RecipeCursor : MonoBehaviour
    {
        [SerializeField] private IngredientTile[] inventoryTiles;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image itemNameImage;
        
        private void Update() => 
            transform.position = Input.mousePosition;
        
        public void SetHoverRecipe(CraftingRecipe recipe)
        {
            ResetTiles();
            SetTiles(recipe);
            SetItemNameVisual(recipe);
        }

        public void ClearHoverRecipe()
        {
            ResetTiles();
            ClearItemNameVisual();
        }

        private void SetTiles(CraftingRecipe recipe)
        {
            for (int i = 0; i < recipe.ingredients.Length; i++) 
                inventoryTiles[i].SetItem(recipe.ingredients[i]);
        }

        private void ResetTiles()
        {
            foreach (var tile in inventoryTiles) 
                tile.ResetTile();
        }

        private void SetItemNameVisual(CraftingRecipe recipe)
        {
            nameText.text = recipe.result.preset.itemName;
            itemNameImage.color = Color.white;
        }

        private void ClearItemNameVisual()
        {
            nameText.text = "";
            itemNameImage.color = Color.clear;
        }
    }
}