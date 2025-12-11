using System;
using System.Collections.Specialized;
using System.Linq;
using Code.Gameplay.Inventory;
using Code.Infrastructure.Inputs;
using Code.UI.Services.Windows;
using Code.UI.Windows;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player.Inventory
{
    public class PlayerInventory : Gameplay.Inventory.Inventory
    {
        private InventoryView inventoryView;
        private IInputService input;
        private WindowBase inventoryWindow;
        private IWindowService windows;

        private bool isWindowOpened;

        [Inject]
        public void Construct(
            IInputService input,
            IWindowService windows)
        {
            this.input = input;
            this.windows = windows;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if (!IsOwner)
                return;
            
            Initialize();
        }

        private void Initialize()
        {
            input.InventoryUIButtonDown += OnInventoryButtonDown;
            
            inventoryWindow = windows.Open(WindowId.Inventory);
            windows.Close(WindowId.Inventory);
            
            inventoryView = ((InventoryWindow)inventoryWindow).inventoryView;
            inventoryView.Initialize(this);
            
            for (int i = 0; i < inventoryView.GetInventoryTilesLength(); i++) 
                inventoryItems.Add(new InventoryItem());
                    
            inventoryItems.CollectionChanged += OnInventoryChanged;
        }

        private void OnInventoryButtonDown()
        {
            if (!isWindowOpened)
            {
                windows.Open(WindowId.Inventory);
                isWindowOpened = true;
                
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                windows.Close(WindowId.Inventory);
                isWindowOpened = false;
                
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public override void Interact(int index) => 
            Debug.Log($"Interact item with index: {index}");

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs args) => 
            inventoryView.RedrawEverything(inventoryItems.ToArray());

        private void OnDestroy() => 
            input.InventoryUIButtonDown -= OnInventoryButtonDown;
    }
}