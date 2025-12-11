using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

namespace Code.Infrastructure.Inputs
{
    public class StandaloneInputService : IInputService, IInitializable
    {
        public PlayerInput Input { get; set; }
        public event Action InventoryUIButtonDown;
        public event Action CollectItemButtonDown;
        public event Action LeftMouseButtonDown;
        public event Action RightMouseButtonDown;

        public void Initialize()
        {
            Input = new PlayerInput();
            Input.Inventory.InventoryUI.started += OnInventoryUIButtonDown;
            Input.Inventory.CollectItem.started += OnCollectItemButtonDown;
            Input.UI.LeftMouseButtonClick.started += OnLeftMouseButtonClick;
            Input.UI.RightMouseButtonClick.started += OnRightMouseButtonClick;
        }

        private void OnInventoryUIButtonDown(InputAction.CallbackContext obj) =>
            InventoryUIButtonDown?.Invoke();

        private void OnCollectItemButtonDown(InputAction.CallbackContext obj) => 
            CollectItemButtonDown?.Invoke();

        private void OnLeftMouseButtonClick(InputAction.CallbackContext obj) => 
            LeftMouseButtonDown?.Invoke();

        private void OnRightMouseButtonClick(InputAction.CallbackContext obj) => 
            RightMouseButtonDown?.Invoke();
    }
}