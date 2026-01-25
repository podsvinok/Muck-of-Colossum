using System;
using Code.Gameplay.Items;
using Code.Utils;
using FishNet.CodeGenerating;
using UnityEngine;

namespace Code.Gameplay.ResourceSystem
{
    [CreateAssetMenu(fileName = "ResourcePreset", menuName = "StaticData/ResourceAsset")]
    public class ResourcePreset : ScriptableObject
    {
        public string resourceName;
        public Resource prefab;
        public Drop[] drop;
        public int rank;
        public ItemType equipmentType;
    }

    [Serializable]
    public struct Drop
    {
        public ItemPreset item;
        public int minAmount;
        public int maxAmount;
        public float dropChance;
    }
}