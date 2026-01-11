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
        private ItemDatabase database;

        private InventoryItem currentActiveSlot;
        private InventoryItem draggedItem; 
        private GameObject holdingItem;
        private bool isHoldingActiveSlot;
        private bool isDragging;
        
        private readonly SyncVar<int> activeSlotId = new(-1);

        [Inject]
        public void Construct(
            IInputService input,
            IWindowService windows,
            ItemDatabase database)
        {
            this.input = input;
            this.windows = windows;
            this.database = database;
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
            TryAddItem(database.TryGetItemPresetByName("1488"), 10);
            TryAddItem(database.TryGetItemPresetByName("NoWorkMuckWolk"), 10);
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
            inventoryView.SlotPointerDown += OnSlotPointerDown;
            inventoryView.SlotPointerEnter += OnSlotPointerEnter;
            inventoryView.SlotPointerExit += OnSlotPointerExit;
            inventoryItems.CollectionChanged += OnInventoryChanged;
            input.InventoryUIButtonDown += OnInventoryButtonDown;
            input.ChangeActiveSlotButtonDown += OnActiveSlotButtonDown;
            input.ChangeActiveSlotScroll += OnActiveSlotScroll;
        }

       

        private void RedrawInventoryViews()
        {
            inventoryView.RedrawEverything(inventoryItems.ToArray());
            actionInventoryView.RedrawEverything(inventoryItems.ToArray());
        }

        private void OnInventoryButtonDown()
        {
            if (windows.IsOpened(WindowId.Inventory))
            {
                windows.CloseAll();
                windows.Open(WindowId.Inventory);
                
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                windows.CloseAll();
                windows.Open(WindowId.InventoryActiveSlots);
                
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

        private void OnActiveSlotScroll(float value)
        {
            int index = activeSlotId.Value + (int)value;
            if (index == activeSlotId.Value) return;
            
            if (index > actionInventoryView.GetActiveSlotsCount() - 1) index = 0;
            if (index < 0) index = actionInventoryView.GetActiveSlotsCount() - 1;
            
            InventoryItem selectedItem = inventoryItems[index];
            RequestChangeItemRPC(index, selectedItem.preset);
        }

        private void OnActiveSlotButtonDown(int slotIndex)
        {
            int index = slotIndex - 1;
            if (index == activeSlotId.Value) return;
            InventoryItem selectedItem = inventoryItems[index];
            RequestChangeItemRPC(index, selectedItem.preset);
        }

        private void OnSlotPointerEnter(int index, InventoryCursor inventoryCursor) => 
            inventoryCursor.SetItemName(inventoryItems[index]);

        private void OnSlotPointerExit(int index, InventoryCursor inventoryCursor) => 
            inventoryCursor.ClearItemNameVisual();

        private void OnSlotPointerDown(int index, InventoryCursor inventoryCursor)
        {
            if (!isDragging)
            {
                var itemInSlot = inventoryItems[index];
                
                if (itemInSlot.preset == null) 
                    return;

                draggedItem = itemInSlot;
                isDragging = true;
                inventoryItems[index] = new InventoryItem();
                
                if (inventoryCursor != null)
                    inventoryCursor.SetHoldingItem(draggedItem);
                return;
            }
            
            var itemInTargetSlot = inventoryItems[index];
            
            if (itemInTargetSlot.preset == null)
            {
                inventoryItems[index] = draggedItem;
                
                draggedItem = new InventoryItem();
                isDragging = false;
                if (inventoryCursor != null) 
                    inventoryCursor.Hide();
                return;
            }
            
            if (CanStack(draggedItem, itemInTargetSlot))
            {
                itemInTargetSlot.quantity += draggedItem.quantity;
                inventoryItems[index] = itemInTargetSlot;
                
                draggedItem = new InventoryItem();
                isDragging = false;
                if (inventoryCursor != null)
                    inventoryCursor.Hide();
                return;
            }
            
            inventoryItems[index] = draggedItem;
            draggedItem = itemInTargetSlot;
            if (inventoryCursor != null) 
                inventoryCursor.SetHoldingItem(draggedItem);
        }

        private bool CanStack(InventoryItem itemA, InventoryItem itemB)
        {
            if (itemA.preset == null || itemB.preset == null) 
                return false;
            return itemA.preset.uid == itemB.preset.uid;
        }
        
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
            inventoryView.SlotPointerDown += OnSlotPointerDown;
            inventoryItems.CollectionChanged -= OnInventoryChanged;
            input.InventoryUIButtonDown -= OnInventoryButtonDown;
            input.ChangeActiveSlotButtonDown -= OnActiveSlotButtonDown;;
        }
    }
}