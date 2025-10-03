using Code.Utils;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public class LoadingCurtainProxy : ILoadingCurtain
    {
        private LoadingCurtain.Factory factory;
        private ILoadingCurtain impl;

        public LoadingCurtainProxy(LoadingCurtain.Factory factory) => 
            this.factory = factory;

        public async UniTask InitializeAsync() => 
            impl = await factory.Create(AssetPath.LoadingCurtain);

        public void Show() => 
            impl.Show();

        public void Hide() => 
            impl.Hide();
    }
}