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
        [SerializeField] private ResourceHud hud;
        private ResourceNetworkService resourceNetworkService;
        private TerrainChunk chunk;
        private int hp = 100;
            
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
            hp -= 10;
            hud.EnableCanvas();
            hud.SetHp(hp / 100f);
            if (hp <= 0)
                BeDestroyed();
        }

        public void BeDestroyedNetwork()
        {
            chunk.DeleteResource(this);
            Destroy(gameObject);
        }
    }
}