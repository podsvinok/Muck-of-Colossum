using System.Collections.Generic;
using Code.Gameplay.Levels;
using Code.Gameplay.TerrainGeneration.Structures;
using Code.Infrastructure.StaticData;
using Cysharp.Threading.Tasks;
using UnityEngine;
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
        
        private IStaticDataService staticData;
        private ILevelDataProvider levelData;
        private HeightMapGenerator heightMapGenerator;
        private MeshGenerator meshGenerator;
        private ColliderGenerator colliderGenerator;
        private CharacterController characterController;

        public TerrainGenerator(
            HeightMapGenerator heightMapGenerator,
            MeshGenerator meshGenerator,
            ColliderGenerator colliderGenerator,
            IStaticDataService staticData,
            ILevelDataProvider levelData)
        {
            this.heightMapGenerator = heightMapGenerator;
            this.meshGenerator = meshGenerator;
            this.colliderGenerator = colliderGenerator;
            this.staticData = staticData;
            this.levelData = levelData;
        }

        public async UniTask RegenerateTerrain()
        {
            foreach (var chunk in terrainChunks) 
                Object.Destroy(chunk.meshObject);

            terrainChunks = new TerrainChunk[staticData.MeshSettings.terrainSizeX * staticData.MeshSettings.terrainSizeY];
            await GenerateChunks(staticData.MeshSettings.terrainSizeX, staticData.MeshSettings.terrainSizeY);
            
            if (characterController == null) 
                characterController = viewer.GetComponent<CharacterController>();
            
            characterController.enabled = false;
            UpdateChunks();
            characterController.enabled = true;
            viewer.position += Vector3.up * 5;
        }

        public async UniTask GenerateTerrain()
        {
            await GenerateChunks(staticData.MeshSettings.terrainSizeX, staticData.MeshSettings.terrainSizeY);
        }

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
                    heightMapGenerator,
                    meshGenerator,
                    currentChunkCoord,
                    levelData.TerrainParent, 
                    bottomFalloff, 
                    topFalloff,
                    leftFalloff, 
                    rightFalloff);
                
                await UniTask.Yield();
            }
        }

        private void UpdateChunks()
        {
            var chunkToBakeMesh = new List<TerrainChunk>();
            
            foreach (var chunk in terrainChunks)
            {
                if (chunk.UpdateTerrainChunk(viewer)) 
                    chunkToBakeMesh.Add(chunk);
            }
            colliderGenerator.GenerateCollider(chunkToBakeMesh);
        }

        public void InitializeChunks(Transform player)
        {
            viewer = player;
            var position = viewer.position;
            var viewerPosition = new Vector2(position.x, position.z);
            UpdateChunks();
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
                UpdateChunks();
            }
        }
    }
}