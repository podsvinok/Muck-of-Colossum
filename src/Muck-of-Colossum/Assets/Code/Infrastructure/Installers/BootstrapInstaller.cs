using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Inputs;
using Code.Infrastructure.SceneManagement;
using Code.Infrastructure.States.Factory;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
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
        }

        private void BindInfrastructureServices()
        {
            Container.BindInterfacesTo<BootstrapInstaller>().FromInstance(this).AsSingle();
        }
        
        private void BindGameplayServices()
        {
            Container.Bind<ILevelDataProvider>().To<LevelDataProvider>().AsSingle();
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
            Container.Bind<IInputService>().To<StandaloneInputService>().AsSingle();
            Container.Bind<IAssetProvider>().To<ResourcesAssetProvider>().AsSingle();
        }

        private void BindStates()
        {
            Container.BindInterfacesAndSelfTo<BootstrapState>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameEnterState>().AsSingle();
        }

        private void BindStateFactory()
        {
            Container.BindInterfacesAndSelfTo<StateFactory>().AsSingle();
        }

        private void BindGameStateMachine()
        {
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();
        }

        private void BindGameFactories()
        {
            Container.Bind<IPlayerFactory>().To<PlayerFactory>().AsSingle();
        }

        public void Initialize()
        {
            Container.Resolve<IGameStateMachine>().Enter<BootstrapState>();
        }
    }
}