using System;
using UnityEngine;

namespace Code.Infrastructure.Inputs
{
    public interface IInputService
    {
        public PlayerInput Input { get; set; }
        public event Action InventoryUIButtonDown;
        public event Action CraftingUIButtonDown;
        public event Action CollectItemButtonDown;
        public event Action LeftMouseButtonDown;
        public event Action RightMouseButtonDown;
        public event Action<int> ChangeActiveSlotButtonDown; 
    }
}