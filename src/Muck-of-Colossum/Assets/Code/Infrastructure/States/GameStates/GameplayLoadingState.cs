using System.Collections.Generic;
using Code.Gameplay.Lobby;
using Code.Gameplay.Player.Factory;
using Code.Infrastructure.SceneManagement;
using Code.Infrastructure.States.StateMachine;
using Code.Utils;
using Cysharp.Threading.Tasks;
using FishNet.Connection;

namespace Code.Infrastructure.States.GameStates
{
    public class GameplayLoadingState : IPayloadState<GameplayLoadingStateEnterArgs>
    {
        private readonly ISceneLoader sceneLoader;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IGameStateMachine stateMachine;
        private readonly IPlayerFactory playerFactory;

        public GameplayLoadingState(
            ISceneLoader sceneLoader,
            ILoadingCurtain loadingCurtain,
            IGameStateMachine stateMachine,
            IPlayerFactory playerFactory)
        {
            this.sceneLoader = sceneLoader;
            this.loadingCurtain = loadingCurtain;
            this.stateMachine = stateMachine;
            this.playerFactory = playerFactory;
        }

        public async UniTask Enter(bool asServer)
        {
            loadingCurtain.Show();
            
            if (asServer)
                sceneLoader.LoadSceneNetwork(AssetPath.GameScene);
            
            await stateMachine.Enter<GameplayLoopState>();
            
            loadingCurtain.Hide();
        }
        
        public async UniTask Enter(GameplayLoadingStateEnterArgs args)
        {
            loadingCurtain.Show();

            if (args.AsServer)
            {
                sceneLoader.LoadSceneNetwork(AssetPath.GameScene);
                
                foreach (var player in args.Players) 
                    playerFactory.SpawnPlayer(player.Connection);
            }
            
            await stateMachine.Enter<GameplayLoopState>();
            
            loadingCurtain.Hide();
        }

        public UniTask Exit() => 
            default;
    }
    
    public class GameplayLoadingStateEnterArgs
    {
        public bool AsServer;
        public List<LobbyPlayer> Players;
    }
}