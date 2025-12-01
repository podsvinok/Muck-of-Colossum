using UnityEngine;

namespace Code.Gameplay.Item.Factory
{
    public interface IItemFactory
    {
        public Item SpawnItem(ItemPreset item, Vector3 at);
    }
}