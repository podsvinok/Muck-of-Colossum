using Code.Gameplay.Items;
using Code.Gameplay.Items.Factory;
using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Code.Gameplay.Recipe;
using Code.Gameplay.ResourceSystem.Factory;
using Code.Gameplay.TerrainGeneration.Generators;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Extensions;
using Code.Infrastructure.Inputs;
using Code.Infrastructure.SceneManagement;
using Code.Infrastructure.States.Factory;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.StaticData;
using Code.Random;
using Code.UI.LoadingCurtain;
using Code.UI.Services.Factory;
using Code.UI.Services.Windows;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Managing;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller, IInitializable
    {
        public override void InstallBindings()
        {
            BindGameplayServices();
            BindNetworkServices();
            BindInputService();
            BindWindowService();
            BindRandomService();
            BindStaticDataService();
            BindInfrastructureServices();
            
            BindAssetProvider();
            BindExtensionsServiceProvider();
            
            BindStateFactory();
            BindGameFactories();
            BindUIFactory();
            
            BindStates();
            BindGameStateMachine();
            
            BindLoadingCurtain();
            BindTerrainGenerators();
            
            BindDatabases();
            BindResourcePool();
        }

        private void BindResourcePool()
        {
            Container
                .BindInterfacesAndSelfTo<ResourcePool>()
                .AsSingle();
        }

        private void BindRandomService()
        {
            Container
                .Bind<IRandomService>()
                .To<RandomService>()
                .AsSingle();
        }

        private void BindWindowService()
        {
            Container
                .Bind<IWindowService>()
                .To<WindowService>()
                .AsSingle();
        }

        private void BindUIFactory()
        {
            Container
                .Bind<IUIFactory>()
                .To<UIFactory>()
                .AsSingle();
        }

        private void BindDatabases()
        {
            Container
                .BindInterfacesAndSelfTo<ItemDatabase>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<RecipeDatabase>()
                .AsSingle();
        }

        private void BindExtensionsServiceProvider()
        {
            Container
                .Bind(typeof(IExtensionsServiceProvider), typeof(IInitializable))
                .To<ExtensionsServiceProvider>()
                .AsSingle();
        }

        private void BindTerrainGenerators()
        {
            Container
                .BindInterfacesAndSelfTo<ColliderGenerator>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<MeshGenerator>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<HeightMapGenerator>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<TerrainGenerator>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<NavMeshGenerator>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ResourceGenerator>()
                .AsSingle();
        }

        private void BindStaticDataService()
        {
            Container
                .Bind<IStaticDataService>()
                .To<StaticDataService>()
                .AsSingle();
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
                .FromInstance(InstanceFinder.NetworkManager)
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
                .BindInterfacesAndSelfTo<StandaloneInputService>()
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

            Container
                .BindInterfacesAndSelfTo<MainMenuLoadingState>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<MainMenuState>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<LobbyLoadingState>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<LobbyState>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<GameplayLoadingState>()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<GameplayLoopState>()
                .AsSingle();
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
            
            Container
                .Bind<IResourceFactory>()
                .To<ResourceFactory>()
                .AsSingle();
            
            Container
                .Bind<IItemFactory>()
                .To<ItemFactory>()
                .AsSingle();
        }

        public void Initialize()
        {
            Container.Resolve<IGameStateMachine>()
                .Enter<BootstrapState>();
        }
    }
}