using Code.Gameplay.Items;
using Code.Gameplay.Recipe;
using Code.Utils;
using Zenject;

namespace Code.Infrastructure.Extensions
{
    public class ExtensionsServiceProvider : IExtensionsServiceProvider, IInitializable
    {
        private readonly ItemDatabase itemDatabase;
        private readonly RecipeDatabase recipeDatabase;

        public ExtensionsServiceProvider(
            ItemDatabase itemDatabase,
            RecipeDatabase recipeDatabase)
        {
            this.itemDatabase = itemDatabase;
            this.recipeDatabase = recipeDatabase;
        }

        public void Initialize()
        {
            ProvideServices();
        }

        public void ProvideServices()
        {
            InventoryDataUtils.Initialize(itemDatabase);
            CraftingDataUtils.Initialize(recipeDatabase);
        }
    }
}