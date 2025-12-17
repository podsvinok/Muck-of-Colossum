using UnityEngine;

namespace Code.Gameplay.Item.Factory
{
    public interface IItemFactory
    {
        public Item SpawnItem(ItemPreset itemPreset, Vector3 at);
        public Item SpawnItem(ItemPreset itemPreset, Vector3 at, Transform parent);
    }
}