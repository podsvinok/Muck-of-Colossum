using Code.Gameplay.TerrainGeneration.Structures;
using Code.Infrastructure.StaticData;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

namespace Code.Gameplay.ResourceSystem.Factory
{
    public class ResourceFactory : IResourceFactory
    {
        private ResourcePool resourcePool;

        public ResourceFactory(ResourcePool resourcePool)
        {
            this.resourcePool = resourcePool;
        }

        public Resource SpawnResourceWithParent(Resource resource, Transform parent, SpawnPointData spawnPointData)
        {
            var resourceObject = CreateResourceWithParent(resource.gameObject, parent.transform, spawnPointData);
            var resourceComponent = resourceObject.GetComponent<Resource>();
            resourcePool.Add(resourceComponent);
            
            return resourceComponent;
        }

        private GameObject CreateResourceWithParent(GameObject prefab, Transform parent, SpawnPointData spawnPointData)
        {
            var obj = Object.Instantiate(prefab, parent);
            
            obj.transform.localPosition = new Vector3(spawnPointData.Position.x, spawnPointData.Position.y, spawnPointData.Position.z);
            obj.transform.localRotation = spawnPointData.Rotation;
            obj.transform.localScale = spawnPointData.Scale;
            
            return obj;
        }
    }
}
