using Code.Gameplay.Recipe;
using FishNet.Serializing;

namespace Code.Utils
{
    public static class CraftingDataUtils
    {
        private static RecipeDatabase database;
        public static void Initialize(RecipeDatabase database)
        {
            CraftingDataUtils.database = database;
        }
        
        public static void WriteCraftingRecipe(this Writer writer, CraftingRecipe recipe)
        {
            if (recipe == null)
            {
                writer.WriteBoolean(false);
                return;
            } 
            writer.WriteBoolean(true);
            writer.WriteString(recipe.uid);
        }

        public static CraftingRecipe ReadCraftingRecipe(this Reader reader)
        {
            bool hasRecipe = reader.ReadBoolean();
            if (!hasRecipe)
                return null;

            var uid = reader.ReadStringAllocated();
            database.TryGetRecipe(uid, out var recipe);
            
            return recipe;
        }
    }
}