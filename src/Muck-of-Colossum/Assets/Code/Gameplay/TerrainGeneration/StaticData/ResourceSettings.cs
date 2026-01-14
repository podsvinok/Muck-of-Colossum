using System;
using Code.Gameplay.ResourceSystem;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.StaticData
{
    [CreateAssetMenu(fileName = "Resource Settings", menuName = "TerrainGenerationSettings/Resource Settings")]
    public class ResourceSettings : UpdatableData
    {
        public ResourceGroup[] resourceGroups;
    }
    
    [Serializable]
    public struct ResourceGroup
    {
        public string groupName;
            
        [Header("Prefabs")]
        public WeightedSpawn[] prefabs;
            
        [Header("Noise Distribution")]
        public NoiseSettings noiseSettings;
        public float spawnThreshold;
            
        [Header("Placement Constraints")]
        public int density;
        public float minHeight;
        public float maxHeight;
            
        [Header("Randomization")]
        public Vector3 randomRotationRange;
        public Vector2 scaleRange;

        public float TotalWeight
        {
            get
            {
                float sum = 0;
                foreach (var item in prefabs) sum += item.weight;
                return sum;
            }
        }

        [Serializable]
        public struct WeightedSpawn
        {
            public ResourcePreset resource;
            [Range(0, 1)] public float weight;
        }
    }
}