using Unity.Mathematics;

namespace Code.Gameplay.TerrainGeneration.Structures
{
    public struct SpawnPointData
    {
        public float3 Position;
        public quaternion Rotation;
        public float3 Scale;
        public int PrefabIndex;
    }
}