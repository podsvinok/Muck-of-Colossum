using Code.UI.Services.Factory;
using Code.UI.Windows;
using UnityEngine;

namespace Code.UI.Services.Windows
{
    public class WindowService : IWindowService
    {
        private readonly IUIFactory uiFactory;
        private WindowBase inventoryWindow;

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
            }
        }
    }
}