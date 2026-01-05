using System;
using Code.Gameplay.Recipe;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Gameplay.Player.Crafting
{
    public class CraftingView : MonoBehaviour
    {
        private PlayerCrafting crafting;
        private RecipeTile[] recipeTiles;
        private bool isInitialized;

        public async UniTask Initialize(PlayerCrafting crafting)
        {
            this.crafting = crafting;
            recipeTiles = await crafting.CreateRecipeTiles(transform);
            crafting.InventoryChanged += OnInventoryChanged;
            isInitialized = true;
        }

        public void Craft(CraftingRecipe recipe) => 
            crafting.TryCraft(recipe);

        private void OnInventoryChanged() => 
            UpdateAvailability();

        public void UpdateAvailability()
        {
            if (!isInitialized) return;
            foreach (var tile in recipeTiles) 
                tile.UpdateAvailability(crafting.CanCraft(tile.recipe));
        }

        private void OnDestroy() => 
            crafting.InventoryChanged -= OnInventoryChanged;
    }
}