using Code.UI.Windows;
using Cysharp.Threading.Tasks;

namespace Code.UI.Services.Factory
{
    public interface IUIFactory
    {
        public WindowBase CreateInventory();
        public UniTask CreateUIRoot();
    }
}