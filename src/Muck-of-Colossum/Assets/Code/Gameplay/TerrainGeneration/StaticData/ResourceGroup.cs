using System;
using Code.Gameplay.ResourceSystem;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.StaticData
{
    [CreateAssetMenu(fileName = "Resource Group", menuName = "StaticData/Resource Group")]
    public class ResourceGroup : ScriptableObject
    {
        public string groupName;
            
        public WeightedSpawn[] prefabs;
            
        public NoiseSettings noiseSettings;
        public float spawnThreshold;
            
        public int density;
        public float minHeight;
        public float maxHeight;
            
        public Vector3 randomRotationRange;
        public Vector2 scaleRange;

        public float digInGround;
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