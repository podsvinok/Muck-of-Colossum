using System.Collections.Generic;
using System.Linq;
using Code.Infrastructure.AssetManagement;
using Code.Utils;
using FishNet.Managing;
using UnityEngine;

namespace Code.Gameplay.Recipe
{
    public class RecipeDatabase
    {
        private Dictionary<string, CraftingRecipe> recipes = new();
        private readonly NetworkManager networkManager;
        private readonly IAssetProvider assets;

        public RecipeDatabase(
            NetworkManager networkManager,
            IAssetProvider assets)
        {
            this.networkManager = networkManager;
            this.assets = assets;
        }

        public void LoadRecipes()
        {
            var loadedRecipes = assets.LoadAll<CraftingRecipe>(AssetPath.CraftingRecipes);
            foreach (var recipe in loadedRecipes)
            {
                if (!recipes.TryAdd(recipe.uid, recipe))
                    Debug.LogError($"Duplicate Recipe UID: {recipe.uid} on {recipe.name}");
            }
        }

        public bool TryGetRecipe(string uid, out CraftingRecipe recipe)
        {
            if (string.IsNullOrEmpty(uid))
            {
                recipe = null;
                return false;
            }
            return recipes.TryGetValue(uid, out recipe);
        }
        
        public CraftingRecipe[] GetAllRecipes() => recipes.Values.ToArray();
    }
}