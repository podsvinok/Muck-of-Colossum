using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.Generators
{
    public class FalloffGenerator
    {
        public static float[,] GenerateFalloffMap(int width, int height, 
            float leftFalloff, float rightFalloff, float topFalloff, float bottomFalloff)
        {
            var map = new float[width, height];

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var nx = x / (float)(width - 1);
                var ny = y / (float)(height - 1);

                var fx = Mathf.Max(leftFalloff * (1 - nx), rightFalloff * nx);
                var fy = Mathf.Max(topFalloff * (1 - ny), bottomFalloff * ny);

                var value = Mathf.Max(fx, fy);

                map[y, x] = Evaluate(value);
            }

            return map;
        }

        private static float Evaluate(float value)
        {
            float a = 3;
            var b = 2.2f;

            return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
        }
    }
}