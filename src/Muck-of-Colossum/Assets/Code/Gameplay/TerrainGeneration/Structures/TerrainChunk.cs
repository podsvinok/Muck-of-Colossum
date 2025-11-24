using Code.Gameplay.Levels;
using Code.Gameplay.TerrainGeneration.Generators;
using Code.Gameplay.TerrainGeneration.StaticData;
using Code.Infrastructure.StaticData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

namespace Code.Gameplay.TerrainGeneration.Structures
{
    public class TerrainChunk
    {
        public GameObject meshObject;
        
        private Vector2 coord;
        private Vector2 sampleCentre;
        private Bounds bounds;

        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;

        private LODInfo[] detailLevels;
        private LODMesh[] lodMeshes;
        private int colliderLODIndex;

        private HeightMap heightMap;
        private int previousLODIndex = -1;

        private MeshSettings meshSettings;
        private Transform viewer;
    
        private readonly HeightMapGenerator heightMapGenerator;
        private readonly MeshGenerator meshGenerator;
        private readonly IStaticDataService staticData;
        private readonly ILevelDataProvider levelData;

        public TerrainChunk(
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

        public void Initialize(Vector2 coord,
            float topFalloff, float bottomFalloff, float leftFalloff, float rightFalloff)
        {
            Profiler.BeginSample("TerrainChunk.Initialize");
            this.coord = coord;
            detailLevels = staticData.MeshSettings.detailLevels;
            colliderLODIndex = staticData.MeshSettings.colliderLODIndex;
            meshSettings = staticData.MeshSettings;
            viewer = levelData.Player;

            sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
            var position = coord * meshSettings.meshWorldSize;
            bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);
        
            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer.material = staticData.TextureSettings.mapMaterial;

            meshObject.transform.position = new Vector3(position.x, 0, position.y);
            meshObject.transform.parent = levelData.TerrainParent;

            heightMap = heightMapGenerator.GenerateHeightMap(
                meshSettings.numVertsPerLine,
                meshSettings.numVertsPerLine,
                sampleCentre,
                topFalloff,
                bottomFalloff,
                leftFalloff,
                rightFalloff);
            
            lodMeshes = new LODMesh[detailLevels.Length];
            for (var i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, meshGenerator);
                lodMeshes[i].CreateMesh(heightMap);

                if (i == colliderLODIndex) 
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
            }
            Profiler.EndSample();
        }

        public void UpdateTerrainChunk()
        {
            var viewerDstFromNearestEdge =
                Mathf.Sqrt(bounds.SqrDistance(new Vector3(viewer.position.x, viewer.position.z)));

            var lodIndex = 0;

            for (var i = 0; i < detailLevels.Length - 1; i++)
                if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                    lodIndex = i + 1;
                else
                    break;

            if (lodIndex != previousLODIndex)
            {
                var lodMesh = lodMeshes[lodIndex];
                previousLODIndex = lodIndex;
                meshFilter.mesh = lodMesh.mesh;
            }
        }
    }
}