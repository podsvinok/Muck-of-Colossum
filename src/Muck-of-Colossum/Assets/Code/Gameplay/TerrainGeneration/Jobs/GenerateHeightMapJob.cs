using Code.Gameplay.TerrainGeneration.Structures;
using GameKit.Dependencies.Utilities.Types;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.Jobs
{
    [BurstCompile]
    public struct GenerateHeightMapJob : IJobParallelFor
    {
        //Output
        public NativeArray<float> HeightMap;

        //Input
        public int Width;
        public int Height;
        public float2 SampleCentre;
        
        public int Seed;
        public float Scale;
        public int Octaves;
        public float Persistence;
        public float Lacunarity;
        public float2 Offset;

        public float FalloffA;
        public float FalloffB;
        public float4 FalloffEdges; // x=left, y=right, z=top, w=bottom

        public float HeightMultiplier;
        [ReadOnly] public NativeArray<Keyframe> CurveData;

        public void Execute(int index)
        {
            //Convert 1D index to x, y
            int x = index % Width;
            int y = index / Width;

            //Noise Generation
            float amplitude = 1;
            float frequency = 1;
            float noiseHeight = 0;
            float maxPossibleHeight = 0;

            var random = new Unity.Mathematics.Random((uint)(Seed + 1));

            for (int i = 0; i < Octaves; i++)
            {
                float offsetX = random.NextFloat(-100000, 100000) + Offset.x + SampleCentre.x;
                float offsetY = random.NextFloat(-100000, 100000) - Offset.y - SampleCentre.y;

                float sampleX = (x - Width / 2f + offsetX) / Scale * frequency;
                float sampleY = (y - Height / 2f + offsetY) / Scale * frequency;

                float perlinValue = noise.cnoise(new float2(sampleX, sampleY)); 
                
                noiseHeight += perlinValue * amplitude;
                maxPossibleHeight += amplitude;

                amplitude *= Persistence;
                frequency *= Lacunarity;
            }

            float normalizedHeight = (noiseHeight + 1) / (maxPossibleHeight / 0.9f);
            float finalValue = math.clamp(normalizedHeight, 0, 1);

            //Falloff Calculation
            float nx = x / (float)(Width - 1);
            float ny = y / (float)(Height - 1);

            float fx = math.max(FalloffEdges.x * (1 - nx), FalloffEdges.y * nx); //Left/Right
            float fy = math.max(FalloffEdges.z * (1 - ny), FalloffEdges.w * ny); //Top/Bottom
            float falloffValue = EvaluateFalloff(math.max(fx, fy), FalloffA, FalloffB);

            finalValue = math.clamp(finalValue - falloffValue, 0, 1);
            HeightMap[index] = finalValue * EvaluateAnimationCurve(CurveData, finalValue) * HeightMultiplier;
        }

        float EvaluateAnimationCurve(NativeArray<Keyframe> curve, float t)
        {
            float value = 0;

            for (int i = 0; i < curve.Length; i++)
            {
                int next = math.clamp(i + 1, 0, curve.Length - 1);
                Keyframe start = curve[i];
                Keyframe end = curve[next];

                int minCheck = math.select(0, 1, t > start.time);
                int maxCheck = math.select(0, 1, t <= end.time);
                int check = minCheck * maxCheck;

                float distanceTime = end.time - start.time;
                
                float m0 = start.outTangent * distanceTime;
                float m1 = end.inTangent * distanceTime;

                float t2 = t * t;
                float t3 = t2 * t;

                float a = 2 * t3 - 3 * t2 + 1;
                float b = t3 - 2 * t2 + t;
                float c = t3 - t2;
                float d = -2 * t3 + 3 * t2;

                value += (a * start.value + b * m0 + c * m1 + d * end.value) * check;
            }
            return value;
        }
        
        private float EvaluateFalloff(float value, float a, float b) => 
            math.pow(value, a) / (math.pow(value, a) + math.pow(b - b * value, a));
    }
}