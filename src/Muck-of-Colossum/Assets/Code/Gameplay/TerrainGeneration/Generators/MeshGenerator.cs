using Code.Gameplay.TerrainGeneration.Jobs;
using Code.Infrastructure.StaticData;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.Generators
{
    public class MeshGenerator
    {
        private readonly IStaticDataService staticData;

        public MeshGenerator(IStaticDataService staticData)
        {
            this.staticData = staticData;
        }

        public Mesh GenerateMesh(int lod, NativeArray<float> heightMap)
        {
            int numVertsPerLine = staticData.MeshSettings.numVertsPerLine;
            int skipIncrement = (lod == 0) ? 1 : lod * 2;
            float topLeftX = (numVertsPerLine - 1) / -2f;
            float topLeftZ = (numVertsPerLine - 1) / 2f;

            var vertices = new NativeList<float3>(numVertsPerLine * numVertsPerLine, Allocator.TempJob);
            var triangles = new NativeList<int>((numVertsPerLine - 1) * (numVertsPerLine - 1) * 6, Allocator.TempJob);
            var uvs = new NativeList<float2>(numVertsPerLine * numVertsPerLine, Allocator.TempJob);
            
            var meshJob = new GenerateMeshJob
            {
                Vertices = vertices,
                Triangles = triangles,
                UVs = uvs,
                
                HeightMap = heightMap,
                NumVertsPerLine = numVertsPerLine,
                SkipIncrement = skipIncrement,
                MeshWorldSize = staticData.MeshSettings.meshWorldSize,
                TopLeftX = topLeftX * staticData.MeshSettings.meshWorldSize / (numVertsPerLine - 3) * 2,
                TopLeftZ = topLeftZ * staticData.MeshSettings.meshWorldSize / (numVertsPerLine - 3) * 2,
            };

            JobHandle meshHandle = meshJob.Schedule();
            meshHandle.Complete();

            var mesh = new Mesh();
            mesh.SetVertices(vertices.AsArray());
            mesh.SetTriangles(triangles.AsArray().ToArray(), 0);
            mesh.SetUVs(0, uvs.AsArray());
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            vertices.Dispose();
            triangles.Dispose();
            uvs.Dispose();
            
            return mesh;
        }
    }
}