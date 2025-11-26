using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Code.Gameplay.TerrainGeneration.Jobs
{
    [BurstCompile]
    public struct GenerateColliderJob : IJobParallelFor
    {
        [ReadOnly] public NativeList<int> MeshIds;
        public void Execute(int index)
        {
            Physics.BakeMesh(MeshIds[index], false, MeshColliderCookingOptions.None);
        }
    }
}