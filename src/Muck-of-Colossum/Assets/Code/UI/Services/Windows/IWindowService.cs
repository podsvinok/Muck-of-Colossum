using Code.UI.Windows;

namespace Code.UI.Services.Windows
{
    public interface IWindowService
    {
        public bool IsOpened(WindowId windowId);
        public WindowBase Open(WindowId windowId);
        public void Close(WindowId windowId);
        public void CloseAll();
    }
}