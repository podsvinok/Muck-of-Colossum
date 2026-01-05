using System.Collections.Generic;
using System.Linq;
using Code.UI.Services.Factory;
using Code.UI.Windows;
using UnityEngine;

namespace Code.UI.Services.Windows
{
    public class WindowService : IWindowService
    {
        private readonly IUIFactory uiFactory;
        private readonly Dictionary<WindowId, WindowBase> windows = new();

        public WindowService(IUIFactory uiFactory)
        {
            this.uiFactory = uiFactory;
        }

        public WindowBase Open(WindowId windowId)
        {
            if (!windows.Keys.Contains(windowId))
                windows[windowId] = uiFactory.CreateWindow(windowId);
            
            windows[windowId].Show();
            return windows[windowId];
        }

        public void CloseAll()
        {
            foreach (var window in windows) 
                Close(window.Key);
        }
        
        public void Close(WindowId windowId) => 
            windows[windowId].Hide();
    }
}