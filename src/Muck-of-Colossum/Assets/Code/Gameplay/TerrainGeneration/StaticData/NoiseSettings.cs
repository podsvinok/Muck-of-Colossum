using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.StaticData
{
    [CreateAssetMenu(fileName = "NoiseSettings", menuName = "TerrainGenerationSettings/NoiseSettings")]
    public class NoiseSettings : UpdatableData
    {
        public float scale = 50;

        public int octaves = 6;
        [Range(0, 1)] public float persistance = .6f;
        public float lacunarity = 2;

        public int seed;
        public Vector2 offset;

        public void ValidateValues()
        {
            scale = Mathf.Max(scale, 0.01f);
            octaves = Mathf.Max(octaves, 1);
            lacunarity = Mathf.Max(lacunarity, 1);
            persistance = Mathf.Clamp01(persistance);
        }
    }

    public struct NoiseSettingsStruct
    {
        public NoiseSettingsStruct(
            float scale,
            int octaves,
            float persistance,
            float lacunarity,
            int seed,
            Vector2 offset)
        {
            this.scale = scale;
            this.octaves = octaves;
            this.persistance = persistance;
            this.lacunarity = lacunarity;
            this.seed = seed;
            this.offset = offset;
        }
        
        public float scale;
        public int octaves;
        public float persistance;
        public float lacunarity;
        public int seed;
        public Vector2 offset;
    }
}