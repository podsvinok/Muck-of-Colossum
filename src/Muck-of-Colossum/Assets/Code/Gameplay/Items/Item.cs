using FishNet.Object;
using UnityEngine;

namespace Code.Gameplay.Items
{
    public class Item : NetworkBehaviour
    {
        [SerializeField] private ItemPreset preset;
        public ItemPreset Preset => preset;
        public int Amount;
    }
}