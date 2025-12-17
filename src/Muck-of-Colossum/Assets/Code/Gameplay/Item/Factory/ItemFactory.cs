using FishNet.Managing;
using FishNet.Object;
using UnityEngine;

namespace Code.Gameplay.Item.Factory
{
    public class ItemFactory : IItemFactory
    {
        private readonly NetworkManager networkManager;

        public ItemFactory(NetworkManager networkManager)
        {
            this.networkManager = networkManager;
        }

        public Item SpawnItem(ItemPreset itemPreset, Vector3 at)
        {
            var itemGameObject = CreateItem(itemPreset, at);
            var networkObject = itemGameObject.GetComponent<NetworkObject>();
            
            networkManager.ServerManager.Spawn(networkObject);

            return itemGameObject;
        }

        public Item SpawnItem(ItemPreset itemPreset, Vector3 at, Transform parent)
        {
            var itemGameObject = CreateItem(itemPreset, at, parent);
            var networkObject = itemGameObject.GetComponent<NetworkObject>();
            
            networkManager.ServerManager.Spawn(networkObject);

            return itemGameObject;
        }

        private Item CreateItem(ItemPreset itemPreset, Vector3 at)
        {
            var item = Object.Instantiate(itemPreset.prefab, at, Quaternion.identity);
            return item;
        }
        
        private Item CreateItem(ItemPreset itemPreset, Vector3 at, Transform parent)
        {
            var item = Object.Instantiate(itemPreset.prefab, at, Quaternion.identity, parent);
            return item;
        }
    }
}