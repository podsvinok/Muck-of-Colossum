using UnityEngine;
using System;
using Code.Gameplay.TerrainGeneration.Structures;
using Code.Utils;
using Zenject;

namespace Code.Gameplay.ResourceSystem
{
    public class Resource : MonoBehaviour
    {
        public ResourcePreset resourcePreset;
        [ReadOnly] public int id;
        [SerializeField] private MeshRenderer meshRenderer;
        private ResourceNetworkService resourceNetworkService;
        private TerrainChunk chunk;
            
        [Inject]
        public void Construct(ResourceNetworkService resourceNetworkService)
        {
            this.resourceNetworkService = resourceNetworkService;
        }

        public void Initialize(TerrainChunk chunk)
        {
            this.chunk = chunk;
        }
        
        public void Hide() => 
            meshRenderer.enabled = false;

        public void Show() => 
            meshRenderer.enabled = true;

        public void GetHit() => 
            resourceNetworkService.HitResource(id);

        public void BeDestroyed() => 
            resourceNetworkService.DestroyResource(id);
        
        public void GetHitNetwork()
        {
            Debug.Log($"got hit {id}");
        }

        public void BeDestroyedNetwork()
        {
            Debug.Log($"destroy {id}");
            chunk.DeleteResource(this);
            Destroy(gameObject);
        }
    }
}