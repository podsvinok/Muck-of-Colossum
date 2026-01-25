using System;
using Code.Gameplay.Player.InventorySystem;
using Code.Gameplay.ResourceSystem;
using Code.Infrastructure.Inputs;
using Code.Utils;
using FishNet.Object;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player
{
    public class PlayerResourceDestroyer : NetworkBehaviour
    {
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private Camera playerCamera; 
        private IInputService input;

        [Inject]
        public void Construct(IInputService input)
        {
            this.input = input;
        }

        public override void OnStartClient()
        {
            if (!IsOwner) return;
            input.LeftMouseButtonDown += TryHitResource;
            input.RightMouseButtonDown += KillResource;
        }

        private void KillResource()
        {
            if (Physics.Raycast(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out var hit, 50,
                    1 << LayerMask.NameToLayer(Layers.Resource)))
                if (hit.transform.parent.TryGetComponent<Resource>(out var resource))
                    resource.BeDestroyed();
        }

        private void TryHitResource()
        {
            if (!inventory.GetActiveSlotItem().preset) return;
            if (Physics.Raycast(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out var hit, 50,
                    1 << LayerMask.NameToLayer(Layers.Resource)))
            {
                if (hit.transform.parent.TryGetComponent<Resource>(out var resource))
                {
                    if (resource.resourcePreset.equipmentType == inventory.GetActiveSlotItem().preset.type
                        && resource.resourcePreset.rank - 1 <= inventory.GetActiveSlotItem().preset.tier)
                    {
                        resource.GetHit();
                    }
                }
            }
        }

        public override void OnStopClient()
        {
            if (!IsOwner) return;
            input.LeftMouseButtonDown -= TryHitResource;
            input.RightMouseButtonDown -= KillResource;
        }
    }
}