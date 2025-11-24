using System;
using Code.Gameplay.TerrainGeneration.Generators;
using Code.Gameplay.TerrainGeneration.StaticData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

namespace Code.Gameplay.TerrainGeneration.Structures
{
    public class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
    
        private readonly int lod;
        private readonly MeshGenerator meshGenerator;
        
        public LODMesh(
            int lod,
            MeshGenerator meshGenerator)
        {
            this.lod = lod;
            this.meshGenerator = meshGenerator;
        }

        public void CreateMesh(HeightMap heightMap)
        {
            Profiler.BeginSample("LODMesh.CreateMesh");
            hasRequestedMesh = true;
            
            mesh = meshGenerator
                .GenerateTerrainMesh(heightMap.Values, lod)
                .CreateMesh();
        
            hasMesh = true;
            Profiler.EndSample();
        }
    }
}