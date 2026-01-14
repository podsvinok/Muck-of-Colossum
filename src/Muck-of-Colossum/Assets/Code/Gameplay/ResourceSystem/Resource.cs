using UnityEngine;
using System;
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
            
        [Inject]
        public void Construct(ResourceNetworkService resourceNetworkService)
        {
            this.resourceNetworkService = resourceNetworkService;
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
            Destroy(gameObject);
        }
    }
}