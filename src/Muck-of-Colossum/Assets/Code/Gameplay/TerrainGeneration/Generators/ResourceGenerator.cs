using System.Collections.Generic;
using Code.Gameplay.TerrainGeneration.Jobs;
using Code.Gameplay.TerrainGeneration.StaticData;
using Code.Gameplay.TerrainGeneration.Structures;
using Code.Infrastructure.StaticData;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.Generators
{
    public class ResourceGenerator
    {
        private readonly IStaticDataService staticData;

        public ResourceGenerator(IStaticDataService staticData)
        {
            this.staticData = staticData;
        }

        public List<GroupSpawnResult> GenerateAllResources(Vector2 coord, NativeArray<float> heightMap)
        {
            var allResults = new List<GroupSpawnResult>();
            var settings = staticData.ResourceSettings;
            var meshSettings = staticData.MeshSettings;
            
            int mapSize = meshSettings.numVertsPerLine * meshSettings.numVertsPerLine;
            var occupiedMap = new NativeArray<bool>(mapSize, Allocator.TempJob);

            for (int g = 0; g < settings.resourceGroups.Length; g++)
            {
                var group = settings.resourceGroups[g];
                
                var prefabWeights = new NativeArray<float>(group.prefabs.Length, Allocator.TempJob);
                for (int i = 0; i < group.prefabs.Length; i++)
                    prefabWeights[i] = group.prefabs[i].weight;

                var spawnPoints = new NativeList<SpawnPointData>(Allocator.TempJob);

                var job = new GenerateScatterPointsJob
                {
                    SpawnPoints = spawnPoints,
                    HeightMap = heightMap,
                    PrefabWeights = prefabWeights,
                    
                    OccupiedMap = occupiedMap,
                    
                    Width = meshSettings.numVertsPerLine,
                    Height = meshSettings.numVertsPerLine,
                    SampleCentre = new float2(coord.x * meshSettings.meshWorldSize / meshSettings.meshScale,
                                              coord.y * meshSettings.meshWorldSize / meshSettings.meshScale),
                    
                    Seed = staticData.NoiseSettings.seed + g,
                    Scale = group.noiseSettings.scale,
                    Octaves = group.noiseSettings.octaves,
                    Persistence = group.noiseSettings.persistance,
                    Lacunarity = group.noiseSettings.lacunarity,
                    Offset = new float2(group.noiseSettings.offset.x, group.noiseSettings.offset.y),

                    Density = group.density,
                    SpawnThreshold = group.spawnThreshold,
                    MinHeight = group.minHeight * staticData.HeightMapSettings.heightMultiplier,
                    MaxHeight = group.maxHeight * staticData.HeightMapSettings.heightMultiplier,
                    
                    MeshWorldSize = meshSettings.meshWorldSize,
                    TopLeftX = (meshSettings.numVertsPerLine - 1) / -2f * meshSettings.meshWorldSize / (meshSettings.numVertsPerLine - 3) * 2,
                    TopLeftZ = (meshSettings.numVertsPerLine - 1) / 2f * meshSettings.meshWorldSize / (meshSettings.numVertsPerLine - 3) * 2,
                    
                    TotalWeight = group.TotalWeight,
                    RotationRange = group.randomRotationRange,
                    ScaleRange = group.scaleRange,
                    
                    OccupancyRadius = Mathf.Max(1, group.density / 2) 
                };

                job.Schedule().Complete();

                allResults.Add(new GroupSpawnResult {
                    GroupIndex = g,
                    Points = spawnPoints.ToArray(Allocator.Persistent)
                });

                prefabWeights.Dispose();
                spawnPoints.Dispose();
            }
            
            occupiedMap.Dispose();

            return allResults;
        }
    }
    public struct GroupSpawnResult
    {
        public int GroupIndex;
        public NativeArray<SpawnPointData> Points;
    }
}
