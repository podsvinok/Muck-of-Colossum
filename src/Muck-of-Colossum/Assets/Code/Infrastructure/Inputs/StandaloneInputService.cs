using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Code.Infrastructure.Inputs
{
    public class StandaloneInputService : IInputService, IInitializable, IDisposable
    {
        public PlayerInput Input { get; set; }
        public event Action InventoryUIButtonDown;
        public event Action CraftingUIButtonDown;
        public event Action CollectItemButtonDown;
        public event Action LeftMouseButtonDown;
        public event Action RightMouseButtonDown;
        public event Action<int> ChangeActiveSlotButtonDown;
        public event Action<float> ChangeActiveSlotScroll;

        public void Initialize()
        {
            Input = new PlayerInput();
            Input.Inventory.InventoryUI.performed += OnInventoryUIButtonDown;
            Input.Interactions.Crafting.performed += OnCraftingButtonDown;
            Input.Inventory.CollectItem.performed += OnCollectItemButtonDown;
            Input.UI.LeftMouseButtonClick.performed += OnLeftMouseButtonClick;
            Input.UI.RightMouseButtonClick.performed += OnRightMouseButtonClick;
            Input.Inventory.ChangeActiveSlot.performed += OnChangeActiveSlotButtonDown;
            Input.Inventory.ChangeActiveSlotWheel.performed += OnChangeActiveSlotWheel;
            Input.Enable();
        }

        private void OnChangeActiveSlotWheel(InputAction.CallbackContext obj) => 
            ChangeActiveSlotScroll?.Invoke(obj.ReadValue<float>());

        private void OnCraftingButtonDown(InputAction.CallbackContext obj) => 
            CraftingUIButtonDown?.Invoke();

        private void OnChangeActiveSlotButtonDown(InputAction.CallbackContext obj) => 
            ChangeActiveSlotButtonDown?.Invoke((int)obj.ReadValue<float>());

        private void OnInventoryUIButtonDown(InputAction.CallbackContext obj) => 
            InventoryUIButtonDown?.Invoke();

        private void OnCollectItemButtonDown(InputAction.CallbackContext obj) => 
            CollectItemButtonDown?.Invoke();

        private void OnLeftMouseButtonClick(InputAction.CallbackContext obj) => 
            LeftMouseButtonDown?.Invoke();

        private void OnRightMouseButtonClick(InputAction.CallbackContext obj) => 
            RightMouseButtonDown?.Invoke();

        public void Dispose()
        {
            Input.Inventory.InventoryUI.started -= OnInventoryUIButtonDown;
            Input.Inventory.CollectItem.started -= OnCollectItemButtonDown;
            Input.UI.LeftMouseButtonClick.started -= OnLeftMouseButtonClick;
            Input.UI.RightMouseButtonClick.started -= OnRightMouseButtonClick;
            Input.Inventory.ChangeActiveSlot.performed -= OnChangeActiveSlotButtonDown;
        }
    }
}