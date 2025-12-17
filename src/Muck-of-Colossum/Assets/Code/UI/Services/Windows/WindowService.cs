using Code.UI.Services.Factory;
using Code.UI.Windows;
using UnityEngine;

namespace Code.UI.Services.Windows
{
    public class WindowService : IWindowService
    {
        private readonly IUIFactory uiFactory;
        private WindowBase inventoryWindow;
        private WindowBase inventoryActiveSlotsWindow;

        public WindowService(IUIFactory uiFactory)
        {
            this.uiFactory = uiFactory;
        }

        public WindowBase Open(WindowId windowId)
        {
            WindowBase openedWindow = null;
            switch (windowId)
            {
                case WindowId.Inventory:
                    if (inventoryWindow == null) 
                        inventoryWindow = uiFactory.CreateInventory();
                    openedWindow = inventoryWindow;
                    inventoryWindow.Show();
                    break;
                
                case WindowId.InventoryActiveSlots:
                    if (inventoryActiveSlotsWindow == null)
                        inventoryActiveSlotsWindow = uiFactory.CreateInventoryActiveSlots();
                    openedWindow = inventoryActiveSlotsWindow;
                    inventoryActiveSlotsWindow.Show();
                    break;
            }

            if (openedWindow == null)
                Debug.LogError($"There's no implementation for WindowId: {windowId}");
            
            return openedWindow;
        }

        public void Close(WindowId windowId)
        {
            switch (windowId)
            {
                case WindowId.Inventory:
                    if (inventoryWindow != null)
                        inventoryWindow.Hide();
                    break;
                
                case WindowId.InventoryActiveSlots:
                    if (inventoryActiveSlotsWindow != null)
                        inventoryActiveSlotsWindow.Hide();
                    break;
            }
        }
    }
}