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

        public NetworkObject SpawnItem(Item item, Vector3 at, int amount)
        {
            var itemGameObject = CreateItem(item.gameObject, at);
            itemGameObject.transform.localScale = new Vector3(3, 3, 3);
            var networkObject = itemGameObject.GetComponent<NetworkObject>();
            var itemComponent = itemGameObject.GetComponent<Item>();
            itemComponent.Amount = amount;
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

        public NetworkObject SpawnItemWithParent(Item item, NetworkObject parent, Vector3 rotation)
        {
            var itemGameObject = CreateItemWithParent(item.gameObject, parent.transform);
            itemGameObject.transform.localRotation = Quaternion.Euler(rotation);
            itemGameObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
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