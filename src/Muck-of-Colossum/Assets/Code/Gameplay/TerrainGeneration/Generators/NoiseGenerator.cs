using System;
using Code.Gameplay.TerrainGeneration.StaticData;
using Code.Infrastructure.StaticData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using Random = System.Random;

namespace Code.Gameplay.TerrainGeneration.Generators
{
    public class NoiseGenerator
    {
        private float[,] noiseMap;
        private Vector2[] octaveOffsets;

        private readonly IStaticDataService staticData;
        private int[] randomInts;

        public NoiseGenerator(IStaticDataService staticData)
        {
            this.staticData = staticData;
        }

        public float[,] GenerateNoiseMap(int mapWidth, int mapHeight, Vector2 sampleCentre)
        {
            Profiler.BeginSample("NoiseGenerator.GenerateNoiseMap");
            
            if (noiseMap == null)
                noiseMap = new float[mapHeight, mapWidth];
            else
                Array.Clear(noiseMap, 0, noiseMap.Length);

            if (octaveOffsets == null)
                octaveOffsets = new Vector2[staticData.NoiseSettings.octaves];
            else
                Array.Clear(octaveOffsets, 0, octaveOffsets.Length);

            if (randomInts == null)
            {
                var prng = new Random(staticData.NoiseSettings.seed);
                randomInts = new int[staticData.NoiseSettings.octaves * 2];
                for (int i = 0; i < randomInts.Length; i++)
                    randomInts[i] = prng.Next(-100000, 100000);
            }

            float maxPossibleHeight = 0;
            float amplitude = 1;

            for (var i = 0; i < staticData.NoiseSettings.octaves; i++)
            {
                var offsetX = randomInts[i] + staticData.NoiseSettings.offset.x + sampleCentre.x;
                var offsetY = randomInts[i + staticData.NoiseSettings.octaves] - staticData.NoiseSettings.offset.y - sampleCentre.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= staticData.NoiseSettings.persistance;
            }

            var maxLocalNoiseHeight = float.MinValue;
            var minLocalNoiseHeight = float.MaxValue;

            var halfWidth = mapWidth / 2f;
            var halfHeight = mapHeight / 2f;

            for (var y = 0; y < mapHeight; y++)
            for (var x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (var i = 0; i < staticData.NoiseSettings.octaves; i++)
                {
                    var sampleX = (x - halfWidth + octaveOffsets[i].x) / staticData.NoiseSettings.scale * frequency;
                    var sampleY = (y - halfHeight + octaveOffsets[i].y) / staticData.NoiseSettings.scale * frequency;

                    var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= staticData.NoiseSettings.persistance;
                    frequency *= staticData.NoiseSettings.lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight) maxLocalNoiseHeight = noiseHeight;
                if (noiseHeight < minLocalNoiseHeight) minLocalNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;

                var normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / 0.9f);
                noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
            }
            
            Profiler.EndSample();
            
            return noiseMap;
        }
    }
}