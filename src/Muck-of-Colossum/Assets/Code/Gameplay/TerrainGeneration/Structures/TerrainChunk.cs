using Code.Gameplay.TerrainGeneration.Generators;
using UnityEngine;
using Code.Infrastructure.StaticData;
using Cysharp.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine.AI;

namespace Code.Gameplay.TerrainGeneration.Structures
{
    public class TerrainChunk
    {
        public GameObject meshObject;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        private NavMeshSurface navMeshSurface;
        private int lodIndex = -1;
        private Bounds bounds;

        private readonly IStaticDataService staticData;
        private readonly Vector2 coord;
        private readonly Mesh[] lodMeshes;

        public TerrainChunk(IStaticDataService staticData, HeightMapGenerator heightMapGenerator, MeshGenerator meshGenerator,
            Vector2 coord, Transform parent, float bottomFalloff, float topFalloff, float leftFalloff, float rightFalloff)
        {
            this.staticData = staticData;
            this.coord = coord;
            
            var position = coord * staticData.MeshSettings.meshWorldSize;
            bounds = new Bounds(position, Vector2.one * staticData.MeshSettings.meshWorldSize);
            
            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshObject.layer = LayerMask.NameToLayer("Ground");
            meshRenderer.material = staticData.TextureSettings.mapMaterial;

            meshObject.transform.position = new Vector3(coord.x * staticData.MeshSettings.meshWorldSize,
                0, coord.y * staticData.MeshSettings.meshWorldSize);
            meshObject.transform.parent = parent;

            var heightMap = heightMapGenerator.GenerateHeightMap(
                coord, leftFalloff, rightFalloff, topFalloff, bottomFalloff);
            
            lodMeshes = new Mesh[this.staticData.MeshSettings.detailLevels.Length];
            for (var i = 0; i < staticData.MeshSettings.detailLevels.Length; i++)
            {
                var mesh = meshGenerator.GenerateMesh(staticData.MeshSettings.detailLevels[i].lod, heightMap);
                lodMeshes[i] = mesh;
            }
            heightMap.Dispose();
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

            if (newLodIndex != lodIndex)
            {
                meshFilter.sharedMesh = lodMeshes[newLodIndex];
                if (lodIndex == 0)
                    meshCollider.sharedMesh = null;
                lodIndex = newLodIndex;
            }
        }
 
        public Mesh GetMeshForBaking()
        {
            return lodMeshes[0];
        }

        public void SetBakedCollider()
        {
            meshCollider.cookingOptions = MeshColliderCookingOptions.None;
            meshCollider.sharedMesh = lodMeshes[0];
        }
    }
}