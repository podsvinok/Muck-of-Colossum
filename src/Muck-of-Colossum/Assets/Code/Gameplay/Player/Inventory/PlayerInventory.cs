using System;
using System.Collections.Specialized;
using System.Linq;
using Code.Gameplay.Inventory;
using Code.Gameplay.Item;
using Code.Infrastructure.Inputs;
using Code.UI.Services.Factory;
using Code.UI.Services.Windows;
using Code.UI.Windows;
using Cysharp.Threading.Tasks;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Player.Inventory
{
    public class PlayerInventory : Gameplay.Inventory.Inventory
    {
        [SerializeField] private Transform itemHolder;
        
        private InventoryView inventoryView;
        private ActionInventoryView actionInventoryView;
        private IInputService input;
        private WindowBase inventoryActiveSlotsWindow;
        private WindowBase inventoryWindow;
        private IWindowService windows;

        private InventoryItem currentActiveSlot;
        private Item.Item holdingItem;
        private bool isWindowOpened;
        private bool isHoldingActiveSlot;
        

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
                windows.Open(WindowId.Inventory);
                windows.Close(WindowId.InventoryActiveSlots);
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

        private void OnActiveSlotButtonDown(float newActiveSlot)
        {
            actionInventoryView.SetActiveSlot((int)newActiveSlot - 1);
            currentActiveSlot = inventoryItems[(int)newActiveSlot - 1];
            DespawnInactiveItem();
            TrySpawnActiveItem();
        }

        private void OnInventoryChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            RedrawInventoryViews();
            
            TryDespawnInactiveItem();
            TrySpawnActiveItem();
        }

        private void TrySpawnActiveItem()
        {
            if (!holdingItem && currentActiveSlot.preset && currentActiveSlot.quantity > 0)
                SpawnItemInHolder(currentActiveSlot.preset);
        }

        private void TryDespawnInactiveItem()
        {
            if (!currentActiveSlot.preset || currentActiveSlot.quantity <= 0)
                DespawnInactiveItem();
        }

        private void DespawnInactiveItem()
        {
            if (holdingItem)
                Despawn(holdingItem);
        }

        public override void Interact(int index) => 
            Debug.Log($"Interact item with index: {index}");

        [ServerRpc(RequireOwnership = false)]
        private void Despawn(GameObject item) => 
            Despawn(item, DespawnType.Destroy);
        
        [ServerRpc(RequireOwnership = false)]
        private void SpawnItemInHolder(ItemPreset item) => 
            itemFactory.SpawnItem(item, itemHolder.position, itemHolder);

        private void OnDestroy()
        {
            if (!IsOwner) return;
            inventoryItems.CollectionChanged -= OnInventoryChanged;
            input.InventoryUIButtonDown -= OnInventoryButtonDown;
            input.ChangeActiveSlotButtonDown -= OnActiveSlotButtonDown;;
        }
    }
}