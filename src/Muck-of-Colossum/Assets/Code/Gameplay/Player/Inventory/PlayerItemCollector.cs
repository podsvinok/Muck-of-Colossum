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
        private IInputService input;
        private ILevelDataProvider levelData;

        [Inject]
        public void Construct(IInputService input, ILevelDataProvider levelData)
        {
            this.input = input;
            this.levelData = levelData;
        }

        private void Update()
        {
            if (!IsOwner)
                return;
            
            if (!input.GetLeftMouseButtonUp())
                return;

            if (!Physics.Raycast(levelData.Player.GetComponentInChildren<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out var hit, 50)) 
                return;
            
            if (hit.collider.TryGetComponent(out Item.Item item) && inventory.TryAddItem(item.Preset, 1))
                Despawn(item);
        }

        [ServerRpc(RequireOwnership = false)]
        private void Despawn(Item.Item item) => 
            Despawn(item.gameObject);
    }
}