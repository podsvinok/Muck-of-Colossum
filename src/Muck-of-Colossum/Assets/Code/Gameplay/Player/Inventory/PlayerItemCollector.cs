using Code.Gameplay.Levels;
using Code.Infrastructure.Inputs;
using FishNet.Object;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player.Inventory
{
    public class PlayerItemCollector : NetworkBehaviour
    {
        [SerializeField] private Gameplay.Inventory.Inventory inventory;
        private Camera playerCamera; 
            
        private IInputService input;
        private ILevelDataProvider levelData;

        [Inject]
        public void Construct(
            IInputService input, 
            ILevelDataProvider levelData)
        {
            this.input = input;
            this.levelData = levelData;
        }

        public override void OnStartClient()
        {
            if (!IsOwner) return;
            playerCamera = levelData.Player.GetComponentInChildren<Camera>();
            input.CollectItemButtonDown += OnCollectItemButtonDown;
        }

        private void OnCollectItemButtonDown()
        {
            if (!Physics.Raycast(playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out var hit, 50)) 
                return;
            
            if (hit.collider.TryGetComponent(out Item.Item item) && inventory.TryAddItem(item.Preset, 1))
                Despawn(item);
        }

        [ServerRpc(RequireOwnership = false)]
        private void Despawn(Item.Item item) => 
            Despawn(item.gameObject, DespawnType.Destroy);

        private void OnDestroy()
        {
            if (!IsOwner) return;
            input.CollectItemButtonDown -= OnCollectItemButtonDown;
        }
    }
}