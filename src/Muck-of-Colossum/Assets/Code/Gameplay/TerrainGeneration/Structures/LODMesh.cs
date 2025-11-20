using System;
using Code.Gameplay.TerrainGeneration.Generators;
using Code.Gameplay.TerrainGeneration.StaticData;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.Structures
{
    public class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
    
        private readonly int lod;
        private readonly MeshGenerator meshGenerator;

        public event Action UpdateCallback;
    
        public LODMesh(
            int lod,
            MeshGenerator meshGenerator)
        {
            this.lod = lod;
            this.meshGenerator = meshGenerator;
        }

        public void CreateMesh(HeightMap heightMap, MeshSettings meshSettings)
        {
            hasRequestedMesh = true;
            
            mesh = meshGenerator
                .GenerateTerrainMesh(heightMap.Values, meshSettings, lod)
                .CreateMesh();

            UpdateCallback?.Invoke();
        
            hasMesh = true;
        }
    }
}