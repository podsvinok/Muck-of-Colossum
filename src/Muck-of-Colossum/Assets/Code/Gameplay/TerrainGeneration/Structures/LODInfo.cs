using Code.Gameplay.TerrainGeneration.StaticData;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.Structures
{
    [System.Serializable]
    public struct LODInfo
    {
        [Range(0, MeshSettings.NumSupportedLoDs - 1)]
        public int lod;
        public float visibleDstThreshold;
        public float SqrVisibleDstThreshold => 
            visibleDstThreshold * visibleDstThreshold;
    }
}