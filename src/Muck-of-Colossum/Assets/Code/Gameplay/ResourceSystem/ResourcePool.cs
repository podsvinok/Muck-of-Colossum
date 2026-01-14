using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay.ResourceSystem.Factory
{
    public class ResourcePool
    {
        private Dictionary<int, Resource> resources = new();
        private int currentId;
        
        public Resource Get(int id) => 
            resources[id];

        public void Add(Resource resource)
        {
            resources[currentId] = resource;
            resource.id = currentId;
            currentId++;
        }
    }
}