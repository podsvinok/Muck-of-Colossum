using System;
using Code.UI.Services.Windows;

namespace Code.UI.Windows
{
    [Serializable]
    public class WindowConfig
    {
        public WindowId WindowId;
        public WindowBase Prefab;
    }
}