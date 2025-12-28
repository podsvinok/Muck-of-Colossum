using UnityEngine;
using UnityEngine.AI;

namespace Code.Gameplay.TerrainGeneration.StaticData
{
    [CreateAssetMenu(fileName = "NavMesh Settings", menuName = "TerrainGenerationSettings/NavMeshSettings")]
    public class NavMeshSettings : UpdatableData
    {
        public NavMeshCollectGeometry geometry;
        public float voxelSize;
        public int tileSize;
    }
}