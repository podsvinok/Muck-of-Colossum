using Code.Utils;
using FishNet.CodeGenerating;
using UnityEngine;

namespace Code.Gameplay.Items
{
    [CreateAssetMenu(fileName = "ItemPreset", menuName = "StaticData/ItemAsset")]
    [UseGlobalCustomSerializer]
    public class ItemPreset : ScriptableObject
    {
        [ReadOnly] public string uid;
        public string itemName;
        public Item prefab;
        public Item visualPrefab;
        public Sprite icon;
        public bool isStackable;
        public int tier;
        public ItemType type;
        
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

    public enum ItemType
    {
        Default,
        Axe,
        Pickaxe
    }
}