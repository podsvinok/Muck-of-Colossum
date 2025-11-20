using Code.Gameplay.TerrainGeneration.StaticData;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.Generators
{
    public class NoiseGenerator
    {
        public float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre)
        {
            var noiseMap = new float[mapWidth, mapHeight];

            var prng = new System.Random(settings.seed);
            var octaveOffsets = new Vector2[settings.octaves];

            float maxPossibleHeight = 0;
            float amplitude = 1;
            float frequency = 1;

            for (var i = 0; i < settings.octaves; i++)
            {
                var offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
                var offsetY = prng.Next(-100000, 100000) - settings.offset.y - sampleCentre.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= settings.persistance;
            }

            var maxLocalNoiseHeight = float.MinValue;
            var minLocalNoiseHeight = float.MaxValue;

            var halfWidth = mapWidth / 2f;
            var halfHeight = mapHeight / 2f;


            for (var y = 0; y < mapHeight; y++)
            for (var x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (var i = 0; i < settings.octaves; i++)
                {
                    var sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    var sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                    var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight) maxLocalNoiseHeight = noiseHeight;
                if (noiseHeight < minLocalNoiseHeight) minLocalNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;

                if (settings.normalizeMode == NormalizeMode.Global)
                {
                    var normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }

            if (settings.normalizeMode == NormalizeMode.Local)
                for (var y = 0; y < mapHeight; y++)
                for (var x = 0; x < mapWidth; x++)
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);

            return noiseMap;
        }

        public enum NormalizeMode
        {
            Local,
            Global
        };
    }
}