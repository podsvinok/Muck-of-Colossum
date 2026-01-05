using System;
using System.Collections.Generic;
using Code.Gameplay.Player.InventorySystem;
using Code.Gameplay.Recipe;
using Code.Infrastructure.Inputs;
using Code.UI.Services.Factory;
using Code.UI.Services.Windows;
using Code.UI.Windows;
using Cysharp.Threading.Tasks;
using FishNet.Object;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player.Crafting
{
    public class PlayerCrafting : NetworkBehaviour
    {
        public Action InventoryChanged;
        
        [SerializeField] private PlayerInventory playerInventory;

        private bool isWindowOpened;
        
        private RecipeDatabase recipeDatabase;
        private IWindowService windows;
        private IUIFactory uiFactory;
        private CraftingView craftingView;
        private IInputService input;

        [Inject]
        public void Construct(
            RecipeDatabase recipeDatabase,
            IUIFactory uiFactory,
            IWindowService windows,
            IInputService input)
        {
            this.recipeDatabase = recipeDatabase;
            this.uiFactory = uiFactory;
            this.windows = windows;
            this.input = input;
        }

        public override async void OnStartClient()
        {
            if (!IsOwner) return;
            await Initialize();
        }

        private async UniTask Initialize()
        {
            craftingView = ((CraftingWindow)windows.Open(WindowId.Crafting)).craftingView;
            windows.Close(WindowId.Crafting);
            await craftingView.Initialize(this);
            
            playerInventory.InventoryChanged += OnInventoryChanged;
            input.CraftingUIButtonDown += OnCraftingButtonDown;
        }

        private void OnCraftingButtonDown()
        {
            if (!isWindowOpened)
            {
                windows.CloseAll();
                windows.Open(WindowId.Crafting);
                windows.Open(WindowId.Inventory);
                isWindowOpened = true;
                
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                windows.Close(WindowId.Crafting);
                windows.Close(WindowId.Inventory);
                windows.Open(WindowId.InventoryActiveSlots);
                isWindowOpened = false;
                
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void OnInventoryChanged() => 
            InventoryChanged?.Invoke();

        public async UniTask<RecipeTile[]> CreateRecipeTiles(Transform transform)
        {
            var recipeTiles = new List<RecipeTile>();
            foreach (var recipe in recipeDatabase.GetAllRecipes())
            {
                var recipeTile = await uiFactory.CreateRecipeTile(transform);
                recipeTile.Initialize(recipe, craftingView);
                recipeTile.UpdateAvailability(CanCraft(recipe));
                recipeTiles.Add(recipeTile);
            }
            return recipeTiles.ToArray();
        }

        public void TryCraft(CraftingRecipe recipe)
        {
            if (!IsOwner) return;
            
            if (playerInventory.HasAllIngredients(recipe.ingredients)) 
                CraftItemRpc(recipe);
        }

        public bool CanCraft(CraftingRecipe recipe)
        {
            return playerInventory.HasAllIngredients(recipe.ingredients);
        }

        [ServerRpc]
        private void CraftItemRpc(CraftingRecipe recipe)
        {
            if (recipe == null) return;
            
            playerInventory.RemoveItems(recipe.ingredients);
            playerInventory.TryAddItem(recipe.result.preset, recipe.result.quantity);
        }

        private void OnDestroy()
        {
            playerInventory.InventoryChanged -= OnInventoryChanged;
            input.RightMouseButtonDown -= OnCraftingButtonDown;
        }
    }
}