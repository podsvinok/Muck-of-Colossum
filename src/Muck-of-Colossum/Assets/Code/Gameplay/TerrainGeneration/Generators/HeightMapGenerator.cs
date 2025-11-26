using Code.Gameplay.TerrainGeneration.Jobs;
using Code.Infrastructure.StaticData;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.Generators
{
    public class HeightMapGenerator
    {
        private IStaticDataService staticData;

        public HeightMapGenerator(IStaticDataService staticData)
        {
            this.staticData = staticData;
        }

        public NativeArray<float> GenerateHeightMap(Vector2 coord,
            float leftFalloff, float rightFalloff, float topFalloff, float bottomFalloff)
        {
            int numVertsPerLine = staticData.MeshSettings.numVertsPerLine;
            
            var heightMap = new NativeArray<float>(numVertsPerLine * numVertsPerLine, Allocator.TempJob);
            var curveData = new NativeArray<Keyframe>(staticData.HeightMapSettings.heightCurve.keys.Length, Allocator.TempJob);
            for (int i = 0; i < curveData.Length; i++) 
                curveData[i] = staticData.HeightMapSettings.heightCurve.keys[i];
            
            var heightJob = new GenerateHeightMapJob
            {
                HeightMap = heightMap,
                
                Width = numVertsPerLine,
                Height = numVertsPerLine,
                SampleCentre = new float2(coord.x * staticData.MeshSettings.meshWorldSize / staticData.MeshSettings.meshScale,
                    coord.y * staticData.MeshSettings.meshWorldSize / staticData.MeshSettings.meshScale),
                Seed = staticData.NoiseSettings.seed,
                Scale = staticData.NoiseSettings.scale,
                Octaves = staticData.NoiseSettings.octaves,
                Persistence = staticData.NoiseSettings.persistance,
                Lacunarity = staticData.NoiseSettings.lacunarity,
                Offset = new float2(staticData.NoiseSettings.offset.x, staticData.NoiseSettings.offset.y),
                FalloffA = 3f,
                FalloffB = 2.2f,
                FalloffEdges = new float4(leftFalloff, rightFalloff, topFalloff, bottomFalloff),
                CurveData = curveData,
                HeightMultiplier = staticData.HeightMapSettings.heightMultiplier
            };
            
            JobHandle heightHandle = heightJob.Schedule(numVertsPerLine * numVertsPerLine, 64);
            heightHandle.Complete();
            
            curveData.Dispose();

            return heightMap;
        }
    }
}