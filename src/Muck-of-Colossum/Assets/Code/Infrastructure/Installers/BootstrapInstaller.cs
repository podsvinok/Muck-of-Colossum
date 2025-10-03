using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Inputs;
using Code.Infrastructure.SceneManagement;
using Code.Infrastructure.States.Factory;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using Code.Network;
using Code.Utils;
using Cysharp.Threading.Tasks;
using FishNet.Managing;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller, IInitializable
    {
        public override void InstallBindings()
        {
            BindGameFactories();
            BindGameStateMachine();
            BindStates();
            BindStateFactory();
            BindGameplayServices();
            BindInfrastructureServices();
            BindNetworkServices();
            BindLoadingCurtain();
            BindInputService();
            BindAssetProvider();
        }

        private void BindLoadingCurtain()
        {
             Container
                .BindFactory<string, UniTask<LoadingCurtain>, LoadingCurtain.Factory>()
                .FromFactory<PrefabFactoryAsync<LoadingCurtain>>();

            Container
                .BindInterfacesAndSelfTo<LoadingCurtainProxy>()
                .AsSingle();
        }

        private void BindNetworkServices()
        {
            Container
                .BindInterfacesAndSelfTo<NetworkManager>()
                .FromComponentInNewPrefabResource(AssetPath.NetworkManager)
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<NetworkExtensions>()
                .AsSingle();
        }

        private void BindInfrastructureServices()
        {
            Container
                .BindInterfacesTo<BootstrapInstaller>()
                .FromInstance(this)
                .AsSingle();
        }
        
        private void BindGameplayServices()
        {
            Container
                .Bind<ILevelDataProvider>()
                .To<LevelDataProvider>()
                .AsSingle();
            
            Container
                .Bind<ISceneLoader>()
                .To<SceneLoader>()
                .AsSingle();
        }

        private void BindInputService()
        {
            Container
                .Bind<IInputService>()
                .To<StandaloneInputService>()
                .AsSingle();
        }

        private void BindAssetProvider()
        {
            Container
                .Bind<IAssetProvider>()
                .To<AssetProvider>()
                .AsSingle();
        }

        private void BindStates()
        {
            Container
                .BindInterfacesAndSelfTo<BootstrapState>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<GameLoadingState>()
                .AsSingle();

            //Container.BindInterfacesAndSelfTo<>();
        }

        private void BindStateFactory()
        {
            Container
                .BindInterfacesAndSelfTo<StateFactory>()
                .AsSingle();
        }

        private void BindGameStateMachine()
        {
            Container
                .BindInterfacesAndSelfTo<GameStateMachine>()
                .AsSingle();
        }

        private void BindGameFactories()
        {
            Container
                .Bind<IPlayerFactory>()
                .To<PlayerFactory>()
                .AsSingle();
        }

        public void Initialize()
        {
            Container
                .Resolve<IGameStateMachine>()
                .Enter<BootstrapState>();
        }
    }
}