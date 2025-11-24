using Code.Gameplay.TerrainGeneration.StaticData;
using Code.Gameplay.TerrainGeneration.Structures;
using Code.Infrastructure.StaticData;
using UnityEngine;
using UnityEngine.Profiling;

namespace Code.Gameplay.TerrainGeneration.Generators
{
    public class HeightMapGenerator
    {
        private readonly NoiseGenerator noiseGenerator;
        private readonly FalloffGenerator falloffGenerator;
        private readonly IStaticDataService staticData;

        public HeightMapGenerator(
            NoiseGenerator noiseGenerator,
            FalloffGenerator falloffGenerator, 
            IStaticDataService staticData)
        {
            this.noiseGenerator = noiseGenerator;
            this.falloffGenerator = falloffGenerator;
            this.staticData = staticData;
        }

        public HeightMap GenerateHeightMap(int width, int height, Vector2 sampleCentre,
            float leftFalloff = 0f, float rightFalloff = 0f, float topFalloff = 0f, float bottomFalloff = 0f)
        {
            Profiler.BeginSample("HeightMapGenerator.GenerateHeightMap");
            
            var values = noiseGenerator
                .GenerateNoiseMap(
                    width,
                    height, 
                    sampleCentre);

            var falloffMap = falloffGenerator
                .GenerateFalloffMap(
                    width,
                    height,
                    leftFalloff, 
                    rightFalloff, 
                    topFalloff,
                    bottomFalloff);

            var minValue = float.MaxValue;
            var maxValue = float.MinValue;

            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                values[i, j] = Mathf.Clamp01(values[i, j] - falloffMap[i, j]);

                values[i, j] *= staticData.HeightMapSettings.heightCurve.Evaluate(values[i, j]) 
                                * staticData.HeightMapSettings.heightMultiplier;

                if (values[i, j] > maxValue) maxValue = values[i, j];
                if (values[i, j] < minValue) minValue = values[i, j];
            }

            Profiler.EndSample();
            
            return new HeightMap(values, minValue, maxValue);
        }
    }
}