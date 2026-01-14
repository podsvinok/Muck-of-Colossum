using Code.Gameplay.TerrainGeneration.Structures;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Code.Gameplay.TerrainGeneration.Jobs
{
    [BurstCompile]
    public struct GenerateScatterPointsJob : IJob
    {
        // Output
        public NativeList<SpawnPointData> SpawnPoints;
        public NativeArray<bool> OccupiedMap; 

        // Input
        [ReadOnly] public NativeArray<float> HeightMap;
        [ReadOnly] public NativeArray<float> PrefabWeights;
        
        public int Width;
        public int Height;
        public float2 SampleCentre;
        
        public int Seed;
        public float Scale;
        public int Octaves;
        public float Persistence;
        public float Lacunarity;
        public float2 Offset;

        public int Density;
        public float SpawnThreshold;
        public float MinHeight;
        public float MaxHeight;
        public float MeshWorldSize;
        public float TopLeftX;
        public float TopLeftZ;
        public float TotalWeight;
        
        public float3 RotationRange;
        public float2 ScaleRange;
        
        public int OccupancyRadius;

        public void Execute()
        {
            var random = new Unity.Mathematics.Random((uint)(Seed + 1234));

            for (int y = 0; y < Height; y += Density)
            {
                for (int x = 0; x < Width; x += Density)
                {
                    int index = y * Width + x;

                    if (OccupiedMap[index]) continue;

                    float terrainHeight = HeightMap[index];
                    bool validHeight = terrainHeight >= MinHeight && terrainHeight <= MaxHeight;
                    
                    if (!validHeight) continue;

                    float noiseValue = GetNoiseValue(x, y);
                    
                    if (noiseValue < SpawnThreshold) continue;

                    float jitterX = random.NextFloat(-Density * 0.4f, Density * 0.4f);
                    float jitterY = random.NextFloat(-Density * 0.4f, Density * 0.4f);

                    float2 percent = new float2(x + jitterX, y + jitterY) / (Width - 1);
                    float2 vertexPosition2D = new float2(TopLeftX, TopLeftZ) + new float2(percent.x, -percent.y) * MeshWorldSize;
                    float finalHeight = terrainHeight; 

                    int prefabIndex = GetWeightedIndex(random.NextFloat(0, TotalWeight));

                    quaternion rotation = quaternion.Euler(
                        math.radians(random.NextFloat(-RotationRange.x, RotationRange.x)),
                        math.radians(random.NextFloat(-RotationRange.y, RotationRange.y)),
                        math.radians(random.NextFloat(-RotationRange.z, RotationRange.z))
                    );

                    float scaleVal = random.NextFloat(ScaleRange.x, ScaleRange.y);
                    float3 scale = new float3(scaleVal, scaleVal, scaleVal);

                    SpawnPoints.Add(new SpawnPointData
                    {
                        Position = new float3(vertexPosition2D.x, finalHeight, vertexPosition2D.y),
                        Rotation = rotation,
                        Scale = scale,
                        PrefabIndex = prefabIndex
                    });
                    
                    MarkOccupied(x, y);
                }
            }
        }

        private void MarkOccupied(int centerX, int centerY)
        {
            for (int y = -OccupancyRadius; y <= OccupancyRadius; y++)
            {
                for (int x = -OccupancyRadius; x <= OccupancyRadius; x++)
                {
                    int targetX = centerX + x;
                    int targetY = centerY + y;

                    if (targetX >= 0 && targetX < Width && targetY >= 0 && targetY < Height)
                    {
                        int index = targetY * Width + targetX;
                        OccupiedMap[index] = true;
                    }
                }
            }
        }

        private float GetNoiseValue(float x, float y)
        {
            float amplitude = 1;
            float frequency = 1;
            float noiseHeight = 0;
            float maxHeight = 0;

            for (int i = 0; i < Octaves; i++)
            {
                float sampleX = (x + SampleCentre.x + Offset.x) / Scale * frequency;
                float sampleY = (y + SampleCentre.y + Offset.y) / Scale * frequency;
                
                float perlinValue = noise.cnoise(new float2(sampleX, sampleY)); 
                
                perlinValue = (perlinValue + 1f) * 0.5f;
                
                noiseHeight += perlinValue * amplitude;
                maxHeight += amplitude;

                amplitude *= Persistence;
                frequency *= Lacunarity;
            }
            
            return noiseHeight / maxHeight;
        }

        private int GetWeightedIndex(float randomValue)
        {
            float currentSum = 0;
            for (int i = 0; i < PrefabWeights.Length; i++)
            {
                currentSum += PrefabWeights[i];
                if (randomValue <= currentSum)
                    return i;
            }
            return 0;
        }
    }
}