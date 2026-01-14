using System.Collections.Generic;
using Code.Gameplay.TerrainGeneration.Jobs;
using Code.Gameplay.TerrainGeneration.Structures;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;

namespace Code.Gameplay.TerrainGeneration.Generators
{
    public class ColliderGenerator
    {
        public async UniTask GenerateCollider(TerrainChunk[] chunksToBakeMesh)
        {
            var meshIds = new NativeList<int>(Allocator.Persistent);

            foreach (var chunk in chunksToBakeMesh) 
                meshIds.Add(chunk.GetMeshForBaking().GetInstanceID());
            
            var colliderJob = new GenerateColliderJob
            {
                MeshIds = meshIds
            };

            var colliderHandle = colliderJob.Schedule(meshIds.Length, 32);

            await UniTask.WaitUntil(() => colliderHandle.IsCompleted);
            colliderHandle.Complete();

            foreach (var chunk in chunksToBakeMesh) 
                chunk.SetBakedCollider(); 
            
            meshIds.Dispose();
        }
    }
}