using System.Collections.Generic;
using Code.Gameplay.ResourceSystem;
using Code.Gameplay.ResourceSystem.Factory;
using Code.Gameplay.TerrainGeneration.Generators;
using UnityEngine;
using Code.Infrastructure.StaticData;
using Cysharp.Threading.Tasks;
using Unity.AI.Navigation;
using Unity.Collections;
using UnityEngine.AI;

namespace Code.Gameplay.TerrainGeneration.Structures
{
    public class TerrainChunk
    {
        public GameObject meshObject;
        private List<Resource> spawnedResources = new();
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        private NavMeshSurface navMeshSurface;
        private int currentLodIndex = -1;
        private Bounds bounds;
        private Vector2 position;
        private Mesh[] lodMeshes;
        
        private readonly IStaticDataService staticData;
        private readonly IResourceFactory resourceFactory;

        public TerrainChunk(
            IStaticDataService staticData,
            IResourceFactory resourceFactory,
            HeightMapGenerator heightMapGenerator,
            MeshGenerator meshGenerator,
            ResourceGenerator resourceGenerator,
            Vector2 coord,
            Transform parent,
            float bottomFalloff, float topFalloff, float leftFalloff, float rightFalloff)
        {
            this.staticData = staticData;
            this.resourceFactory = resourceFactory;

            SetPosition(staticData, coord);
            SetBounds(staticData, position);
            
            CreateGameObject();
            PrepareMesh(staticData, coord, parent);

            var heightMap = GenerateHeightMap(heightMapGenerator, coord, bottomFalloff, topFalloff, leftFalloff, rightFalloff);
            
            GenerateLodMeshes(staticData, meshGenerator, heightMap);
            GenerateObjects(resourceGenerator, coord, heightMap);
            
            heightMap.Dispose();
        }

        private void GenerateObjects(ResourceGenerator resourceGenerator, Vector2 coord, NativeArray<float> heightMap)
        {
            List<GroupSpawnResult> results = resourceGenerator.GenerateAllResources(coord, heightMap);

            foreach (var result in results)
            {
                var groupSettings = staticData.ResourceSettings.resourceGroups[result.GroupIndex];
        
                foreach (var point in result.Points)
                {
                    var obj = resourceFactory
                        .SpawnResourceWithParent(
                            groupSettings.prefabs[point.PrefabIndex].resource.prefab,
                            meshObject.transform,
                            point);
            
                    spawnedResources.Add(obj);
                }
            }
        }

        private void SetBounds(IStaticDataService staticData, Vector2 position) => 
            bounds = new Bounds(position, Vector2.one * staticData.MeshSettings.meshWorldSize);

        private void SetPosition(IStaticDataService staticData, Vector2 coord) =>
            position = coord * staticData.MeshSettings.meshWorldSize;

        private NativeArray<float> GenerateHeightMap(HeightMapGenerator heightMapGenerator, Vector2 coord,
            float bottomFalloff, float topFalloff, float leftFalloff, float rightFalloff) => 
            heightMapGenerator.GenerateHeightMap(coord, leftFalloff, rightFalloff, topFalloff, bottomFalloff);

        private void GenerateLodMeshes(IStaticDataService staticData, MeshGenerator meshGenerator, NativeArray<float> heightMap)
        {
            lodMeshes = new Mesh[this.staticData.MeshSettings.detailLevels.Length];
            for (var i = 0; i < staticData.MeshSettings.detailLevels.Length; i++)
            {
                var mesh = meshGenerator.GenerateMesh(staticData.MeshSettings.detailLevels[i].lod, heightMap);
                lodMeshes[i] = mesh;
            }
        }

        private void PrepareMesh(IStaticDataService staticData, Vector2 coord, Transform parent)
        {
            meshRenderer.material = staticData.TextureSettings.mapMaterial;
            meshObject.layer = LayerMask.NameToLayer("Ground");
            meshObject.transform.position = new Vector3(coord.x * staticData.MeshSettings.meshWorldSize,
                0, coord.y * staticData.MeshSettings.meshWorldSize);
            meshObject.transform.parent = parent;
        }

        private void CreateGameObject()
        {
            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
        }

        public void Update(Vector3 viewerPosition)
        {
            var viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(new Vector3(viewerPosition.x, viewerPosition.z)));
            
            int newLodIndex = 0;
            for (var i = 0; i < staticData.MeshSettings.detailLevels.Length - 1; i++)
            {
                if (viewerDstFromNearestEdge > staticData.MeshSettings.detailLevels[i].visibleDstThreshold)
                    newLodIndex = i + 1;
                else
                    break;
            }

            if (newLodIndex != currentLodIndex)
            {
                if (newLodIndex > staticData.MeshSettings.detailLevels[0].lod)
                {
                    foreach (var resource in spawnedResources) 
                        resource.Hide();
                }
                else
                {
                    foreach (var resource in spawnedResources) 
                        resource.Show();
                }
                
                meshFilter.sharedMesh = lodMeshes[newLodIndex];
                currentLodIndex = newLodIndex;
            }
        }
 
        public Mesh GetMeshForBaking() => 
            lodMeshes[0];
        
        public void SetBakedCollider()
        {
            meshCollider.cookingOptions = MeshColliderCookingOptions.None;
            meshCollider.sharedMesh = lodMeshes[0];
        }
    }
}