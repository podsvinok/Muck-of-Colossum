using FishNet.Object;
using UnityEngine;

namespace Code.Gameplay.Item
{
    public class Item : NetworkBehaviour
    {
        [SerializeField] private ItemPreset preset;
        public ItemPreset Preset => preset;
    }
}