using Code.Gameplay.TerrainGeneration.StaticData;
using Code.Gameplay.TerrainGeneration.Structures;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.Generators
{
    public class HeightMapGenerator
    {
        private readonly NoiseGenerator noiseGenerator;

        public HeightMapGenerator(NoiseGenerator noiseGenerator)
        {
            this.noiseGenerator = noiseGenerator;
        }

        public HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre,
            float leftFalloff = 0f, float rightFalloff = 0f, float topFalloff = 0f, float bottomFalloff = 0f)
        {
            var values = noiseGenerator.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);

            var heightCurveThreadSafe = new AnimationCurve(settings.heightCurve.keys);

            var falloffMap = FalloffGenerator
                .GenerateFalloffMap(width, height, leftFalloff, rightFalloff, topFalloff, bottomFalloff);

            var minValue = float.MaxValue;
            var maxValue = float.MinValue;

            for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
            {
                values[i, j] = Mathf.Clamp01(values[i, j] - falloffMap[i, j]);

                values[i, j] *= heightCurveThreadSafe.Evaluate(values[i, j]) * settings.heightMultiplier;

                if (values[i, j] > maxValue) maxValue = values[i, j];
                if (values[i, j] < minValue) minValue = values[i, j];
            }

            return new HeightMap(values, minValue, maxValue);
        }
    }
}