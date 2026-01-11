using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

namespace Code.Gameplay.Items.Factory
{
    public class ItemFactory : IItemFactory
    {
        private readonly NetworkManager networkManager;

        public ItemFactory(NetworkManager networkManager)
        {
            this.networkManager = networkManager;
        }

        public NetworkObject SpawnItem(Item item, Vector3 at)
        {
            var itemGameObject = CreateItem(item.gameObject, at);
            var networkObject = itemGameObject.GetComponent<NetworkObject>();
            networkManager.ServerManager.Spawn(networkObject);
            
            return networkObject;
        }

        public NetworkObject SpawnItemWithParent(Item item, NetworkObject parent)
        {
            var itemGameObject = CreateItemWithParent(item.gameObject, parent.transform);
            var networkObject = itemGameObject.GetComponent<NetworkObject>();
            
            networkObject.SetParent(parent);
            networkManager.ServerManager.Spawn(networkObject);
            
            return networkObject;
        }

        private GameObject CreateItem(GameObject prefab, Vector3 at) => 
            Object.Instantiate(prefab, at, Quaternion.identity);

        private GameObject CreateItemWithParent(GameObject prefab, Transform parent) => 
            Object.Instantiate(prefab, parent);
    }
}