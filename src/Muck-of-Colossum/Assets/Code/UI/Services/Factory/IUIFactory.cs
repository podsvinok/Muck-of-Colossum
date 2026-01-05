using Code.Gameplay.Recipe;
using Code.UI.Services.Windows;
using Code.UI.Windows;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.UI.Services.Factory
{
    public interface IUIFactory
    {
        public WindowBase CreateWindow(WindowId windowId);
        public UniTask CreateUIRoot();
        public UniTask<RecipeTile> CreateRecipeTile(Transform transform);
    }
}