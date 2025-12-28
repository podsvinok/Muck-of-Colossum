using System.Collections.Generic;
using Code.Infrastructure.StaticData;
using Cysharp.Threading.Tasks;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Gameplay.TerrainGeneration.Generators
{
    public class NavMeshGenerator
    {
        private readonly IStaticDataService staticData;
        private NavMeshSurface navMeshSurface;

        public NavMeshGenerator(IStaticDataService staticData)
        {
            this.staticData = staticData;
        }

        public void GenerateNavMesh(Transform terrainParent)
        {
            navMeshSurface = terrainParent.AddComponent<NavMeshSurface>();
            
            navMeshSurface.collectObjects = CollectObjects.Children;
            navMeshSurface.useGeometry = staticData.NavMeshSettings.geometry;
            
            navMeshSurface.overrideVoxelSize = true;
            navMeshSurface.overrideTileSize = true;
            navMeshSurface.voxelSize = staticData.NavMeshSettings.voxelSize;
            navMeshSurface.tileSize = staticData.NavMeshSettings.tileSize;
            
            navMeshSurface.BuildNavMesh();
        }
    }
}