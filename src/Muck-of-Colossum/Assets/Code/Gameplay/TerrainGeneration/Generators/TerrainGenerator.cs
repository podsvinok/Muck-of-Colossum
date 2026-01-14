using System.Collections.Generic;
using Code.Gameplay.Levels;
using Code.Gameplay.ResourceSystem.Factory;
using Code.Gameplay.TerrainGeneration.Structures;
using Code.Infrastructure.StaticData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Zenject;
using Object = UnityEngine.Object;

namespace Code.Gameplay.TerrainGeneration.Generators
{
    public class TerrainGenerator : IFixedTickable
    {
        private const float ViewerMoveThresholdForChunkUpdate = 25f;
        private const float SqrViewerMoveThresholdForChunkUpdate = 
            ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate;
        
        private Vector2 viewerPositionOld;
        private Transform viewer;
        private TerrainChunk[] terrainChunks;
        
        private readonly IStaticDataService staticData;
        private readonly ILevelDataProvider levelData;
        private readonly HeightMapGenerator heightMapGenerator;
        private readonly MeshGenerator meshGenerator;
        private readonly ColliderGenerator colliderGenerator;
        private readonly NavMeshGenerator navMeshGenerator;
        private readonly ResourceGenerator resourceGenerator;
        private readonly IResourceFactory resourceFactory;

        public TerrainGenerator(
            HeightMapGenerator heightMapGenerator,
            MeshGenerator meshGenerator,
            ColliderGenerator colliderGenerator,
            IStaticDataService staticData,
            ILevelDataProvider levelData,
            NavMeshGenerator navMeshGenerator,
            ResourceGenerator resourceGenerator, 
            IResourceFactory resourceFactory)
        {
            this.heightMapGenerator = heightMapGenerator;
            this.meshGenerator = meshGenerator;
            this.colliderGenerator = colliderGenerator;
            this.staticData = staticData;
            this.levelData = levelData;
            this.navMeshGenerator = navMeshGenerator;
            this.resourceGenerator = resourceGenerator;
            this.resourceFactory = resourceFactory;
        }

        public async UniTask RegenerateTerrain()
        {
            foreach (var chunk in terrainChunks) 
                Object.Destroy(chunk.meshObject);

            terrainChunks = new TerrainChunk[staticData.MeshSettings.terrainSizeX * staticData.MeshSettings.terrainSizeY];
            await GenerateChunks(staticData.MeshSettings.terrainSizeX, staticData.MeshSettings.terrainSizeY);
            UpdateChunks(new Vector3(0, 0, 0));
        }

        public async UniTask GenerateTerrain() => 
            await GenerateChunks(staticData.MeshSettings.terrainSizeX, staticData.MeshSettings.terrainSizeY);

        private async UniTask GenerateChunks(int width, int height)
        {
            terrainChunks = new TerrainChunk[staticData.MeshSettings.terrainSizeX 
                                             * staticData.MeshSettings.terrainSizeY];
            var halfWidth = width / 2;
            var halfHeight = height / 2;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var currentChunkCoord = new Vector2(x - halfWidth, y - halfHeight);

                float bottomFalloff = (y == 0) ? 1 : 0;
                float topFalloff = (y == height - 1) ? 1 : 0;
                float leftFalloff = (x == 0) ? 1 : 0;
                float rightFalloff = (x == width - 1) ? 1 : 0;
                
                terrainChunks[y + width * x] = new TerrainChunk(
                    staticData, 
                    resourceFactory,
                    heightMapGenerator,
                    meshGenerator,
                    resourceGenerator,
                    currentChunkCoord,
                    levelData.TerrainParent,
                    bottomFalloff, 
                    topFalloff,
                    leftFalloff, 
                    rightFalloff);
                
                await UniTask.Yield();
            }
        }

        private void UpdateChunks(Vector3 updatePoint)
        {
            foreach (var chunk in terrainChunks) 
                chunk.Update(updatePoint);
        }

        public async UniTask InitializeChunks() 
        {
            UpdateChunks(levelData.StartPoint);
            
            await colliderGenerator.GenerateCollider(terrainChunks);
            navMeshGenerator.GenerateNavMesh(levelData.TerrainParent);
        }

        public void InitializeViewer(Transform viewer)
        {
            this.viewer = viewer;
            UpdateChunks(viewer.position);
        }
        
        public void FixedTick()
        {
            if (viewer == null)
                return;
            
            var position = viewer.position;
            var viewerPosition = new Vector2(position.x, position.z);
            if ((viewerPositionOld - viewerPosition).sqrMagnitude > SqrViewerMoveThresholdForChunkUpdate) 
            {
                viewerPositionOld = viewerPosition;
                UpdateChunks(position);
            }
        }
    }
}