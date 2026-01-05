using System;
using System.Collections.Specialized;
using System.Linq;
using Code.Gameplay.InventorySystem;
using Code.Gameplay.Items;
using Code.Infrastructure.Inputs;
using Code.UI.Services.Factory;
using Code.UI.Services.Windows;
using Code.UI.Windows;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player.InventorySystem
{
    public class PlayerInventory : Inventory
    {
        public Action InventoryChanged;
        
        [SerializeField] private NetworkObject itemHolder;
        
        private InventoryView inventoryView;
        private ActionInventoryView actionInventoryView;
        private WindowBase inventoryActiveSlotsWindow;
        private WindowBase inventoryWindow;
        
        private IInputService input;
        private IWindowService windows;

        private InventoryItem currentActiveSlot;
        private GameObject holdingItem;
        private bool isWindowOpened;
        private bool isHoldingActiveSlot;
        
        private readonly SyncVar<int> activeSlotId = new(-1);

        [Inject]
        public void Construct(
            IInputService input,
            IWindowService windows,
            IUIFactory uiFactory)
        {
            this.input = input;
            this.windows = windows;
        }

        public override void OnStartClient()
        {
            if (IsServerInitialized || IsOwner)
                activeSlotId.OnChange += OnActiveSlotChanged;
            if (!IsOwner) return;
            Initialize();
        }

        private void Initialize()
        {
            InitializeInventoryView();
            InitializeActionInventoryView();

            SetInventoryItems();
            RedrawInventoryViews();
            OnActiveSlotButtonDown(1);
            
            SubscribeToEvents();
        }

        private void InitializeInventoryView()
        {
            inventoryWindow = windows.Open(WindowId.Inventory);
            windows.Close(WindowId.Inventory);
            inventoryView = ((InventoryWindow)inventoryWindow).inventoryView;
            inventoryView.Initialize(this);
        }

        private void InitializeActionInventoryView()
        {
            inventoryActiveSlotsWindow = windows.Open(WindowId.InventoryActiveSlots);
            actionInventoryView = ((ActionInventoryWindow)inventoryActiveSlotsWindow).actionInventoryView;
            actionInventoryView.Initialize();
        }

        private void SetInventoryItems()
        {
            for (int i = 0; i < inventoryView.GetInventoryTilesLength(); i++)
                inventoryItems.Add(new InventoryItem());
        }

        private void SubscribeToEvents()
        {
            inventoryItems.CollectionChanged += OnInventoryChanged;
            input.InventoryUIButtonDown += OnInventoryButtonDown;
            input.ChangeActiveSlotButtonDown += OnActiveSlotButtonDown;
        }

        private void RedrawInventoryViews()
        {
            inventoryView.RedrawEverything(inventoryItems.ToArray());
            actionInventoryView.RedrawEverything(inventoryItems.ToArray());
        }

        private void OnInventoryButtonDown()
        {
            if (!isWindowOpened)
            {
                windows.CloseAll();
                windows.Open(WindowId.Inventory);
                isWindowOpened = true;
                
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                windows.Close(WindowId.Inventory);
                windows.Open(WindowId.InventoryActiveSlots);
                isWindowOpened = false;
                
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            RedrawInventoryViews();
            currentActiveSlot = inventoryItems[activeSlotId.Value];
            RequestChangeItemRPC(activeSlotId.Value, currentActiveSlot.preset);
            InventoryChanged?.Invoke();
        }

        private void OnActiveSlotChanged(int prev, int next, bool asServer)
        {
            if (IsOwner) 
                actionInventoryView.SetActiveSlot(next);
        }

        private void OnActiveSlotButtonDown(int slotIndex)
        {
            int index = slotIndex - 1;
            if (index == activeSlotId.Value) return;
            InventoryItem selectedItem = inventoryItems[index];
            RequestChangeItemRPC(index, selectedItem.preset);
        }

        public override void Interact(int index) => 
            Debug.Log($"Interact item with index: {index}");

        [ServerRpc]
        private void RequestChangeItemRPC(int newIndex, ItemPreset preset)
        {
            activeSlotId.Value = newIndex;

            if (holdingItem != null)
            {
                ServerManager.Despawn(holdingItem);
                holdingItem = null;
            }

            if (preset != null) 
                holdingItem = itemFactory
                    .SpawnItemWithParent(preset.visualPrefab, itemHolder)
                    .gameObject;
        }

        private void OnDestroy()
        {
            if (IsServerOnlyInitialized || IsOwner)
                activeSlotId.OnChange -= OnActiveSlotChanged;
            if (!IsOwner) return;
            inventoryItems.CollectionChanged -= OnInventoryChanged;
            input.InventoryUIButtonDown -= OnInventoryButtonDown;
            input.ChangeActiveSlotButtonDown -= OnActiveSlotButtonDown;;
        }
    }
}