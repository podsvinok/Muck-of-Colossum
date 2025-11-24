using System;
using System.Collections.Generic;
using Code.Gameplay.Levels;
using Code.Gameplay.TerrainGeneration.Structures;
using Code.Infrastructure.StaticData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
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
        
        private List<TerrainChunk> terrainChunks = new();
        
        private HeightMapGenerator heightMapGenerator;
        private IStaticDataService staticData;
        private ILevelDataProvider levelData;
        private MeshGenerator meshGenerator;

        [Inject]
        public void Construct(
            HeightMapGenerator heightMapGenerator,
            MeshGenerator meshGenerator,
            IStaticDataService staticData,
            ILevelDataProvider levelData)
        {
            this.heightMapGenerator = heightMapGenerator;
            this.meshGenerator = meshGenerator;
            this.staticData = staticData;
            this.levelData = levelData;
        }

        public async void RegenerateTerrain()
        {
            foreach (var chunk in terrainChunks)
            {
                Object.Destroy(chunk.meshObject.gameObject);
            }

            terrainChunks = new();
            staticData.LoadTerrainGenerationSettings();
            await GenerateChunks(staticData.MeshSettings.terrainSizeX, staticData.MeshSettings.terrainSizeY);
        }

        public async UniTask GenerateTerrain()
        {
            Profiler.BeginSample("TerrainGenerator.GenerateTerrain");
            viewer = levelData.Player;
            await GenerateChunks(staticData.MeshSettings.terrainSizeX, staticData.MeshSettings.terrainSizeY);
            Profiler.EndSample();
        }

        private async UniTask GenerateChunks(int width, int height)
        {
            var halfWidth = width / 2;
            var halfHeight = height / 2;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var currentChunkCoord = new Vector2(x - halfWidth, y - halfHeight);

                var newChunk = new TerrainChunk(heightMapGenerator, meshGenerator, staticData, levelData);
                terrainChunks.Add(newChunk);
                
                float bottomFalloff = (y == 0) ? 1 : 0;
                float topFalloff = (y == height - 1) ? 1 : 0;
                float leftFalloff = (x == 0) ? 1 : 0;
                float rightFalloff = (x == width - 1) ? 1 : 0;

                newChunk.Initialize(currentChunkCoord, topFalloff, bottomFalloff, leftFalloff, rightFalloff);
                await UniTask.Yield();
            }
            UpdateChunks();
        }

        private void UpdateChunks()
        {
            for (var i = terrainChunks.Count - 1; i >= 0; i--) 
                terrainChunks[i].UpdateTerrainChunk();
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