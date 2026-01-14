using Code.Gameplay.TerrainGeneration.Structures;
using FishNet.Object;
using UnityEngine;

namespace Code.Gameplay.ResourceSystem.Factory
{
    public interface IResourceFactory
    {
        public Resource SpawnResourceWithParent(Resource resource, Transform parent, SpawnPointData spawnPointData);
    }
}