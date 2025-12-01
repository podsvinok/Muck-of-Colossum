using Code.Gameplay.TerrainGeneration.Structures;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.StaticData
{
    [CreateAssetMenu(fileName = "MeshSettings", menuName = "TerrainGenerationSettings/MeshSettings")]
    public class MeshSettings : UpdatableData
    {
        public const int NumSupportedLoDs = 5;
        public const int NumSupportedChunkSizes = 9;
        public const int NumSupportedFlatshadedChunkSizes = 3;
        public static readonly int[] SupportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

        public float meshScale = 2.5f;
        public bool useFlatShading;

        [Range(0, NumSupportedChunkSizes - 1)] 
        public int chunkSizeIndex;

        [Range(0, NumSupportedFlatshadedChunkSizes - 1)]
        public int flatshadedChunkSizeIndex;

        public int terrainSizeX;
        public int terrainSizeY;
        
        public int colliderLODIndex;
        public LODInfo[] detailLevels;
        
        // num verts per line of mesh rendered at LOD = 0. Includes the 2 extra verts that are excluded from final mesh, but used for calculating normals
        public int numVertsPerLine => 
            SupportedChunkSizes[useFlatShading ? flatshadedChunkSizeIndex : chunkSizeIndex] + 5;
        public float meshWorldSize => 
            (numVertsPerLine - 3) * meshScale;
    }
}