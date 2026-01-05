using Code.Gameplay.InventorySystem;
using Code.Gameplay.Items;
using Code.Utils;
using FishNet.CodeGenerating;
using UnityEngine;

namespace Code.Gameplay.Recipe
{
    [UseGlobalCustomSerializer]
    [CreateAssetMenu(fileName = "CraftingRecipe", menuName = "ScriptableObjects/CraftingRecipe")]
    public class CraftingRecipe : ScriptableObject
    {
        [ReadOnly] public string uid;
        public string recipeName;
        public InventoryItem[] ingredients;
        public InventoryItem result;

#if UNITY_EDITOR
        private void OnValidate()
        {
            var assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);

            if (assetPath == null)
            {
                uid = string.Empty;
                return;
            }

            var assetGuid = UnityEditor.AssetDatabase.GUIDFromAssetPath(assetPath).ToString();
            if (string.IsNullOrEmpty(uid) || uid != assetGuid)
                uid = assetGuid;
        }
#endif
    }
}