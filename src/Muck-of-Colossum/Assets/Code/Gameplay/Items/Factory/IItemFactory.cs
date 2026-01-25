using FishNet.Object;
using UnityEngine;

namespace Code.Gameplay.Items.Factory
{
    public interface IItemFactory
    {
        public NetworkObject SpawnItem(Item item, Vector3 at, int amount);
        public NetworkObject SpawnItemWithParent(Item item, NetworkObject parent);
        public NetworkObject SpawnItemWithParent(Item item, NetworkObject parent, Vector3 rotation);
    }
}