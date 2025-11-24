using System.Collections.Generic;
using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Code.Gameplay.TerrainGeneration.Generators;
using Code.Infrastructure.SceneManagement;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.StaticData;
using Code.Network;
using Code.UI.LoadingCurtain;
using Code.Utils;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing;
using Unity.Cinemachine;
using UnityEngine;

namespace Code.Infrastructure.States.GameStates
{
    public class GameplayLoadingState : IPayloadState<GameplayLoadingStateEnterArgs>
    {
        private readonly ISceneLoader sceneLoader;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IGameStateMachine stateMachine;
        private readonly IPlayerFactory playerFactory;
        private readonly IStaticDataService staticData;
        private readonly TerrainGenerator terrainGenerator;
        private readonly ILevelDataProvider levelData;
        private readonly NetworkManager networkManager;

        public GameplayLoadingState(
            ISceneLoader sceneLoader,
            ILoadingCurtain loadingCurtain,
            IGameStateMachine stateMachine,
            IPlayerFactory playerFactory,
            IStaticDataService staticData,
            TerrainGenerator terrainGenerator, 
            ILevelDataProvider levelData, 
            NetworkManager networkManager)
        {
            this.sceneLoader = sceneLoader;
            this.loadingCurtain = loadingCurtain;
            this.stateMachine = stateMachine;
            this.playerFactory = playerFactory;
            this.staticData = staticData;
            this.terrainGenerator = terrainGenerator;
            this.levelData = levelData;
            this.networkManager = networkManager;
        }
        
        public async UniTask Enter(GameplayLoadingStateEnterArgs args)
        {
            loadingCurtain.Show();

            var loadStaticDataTask = staticData.LoadTerrainGenerationSettings();
            
            if (args.AsServer)
            {
                sceneLoader.LoadSceneNetwork(Scenes.GameScene);
                networkManager.SceneManager.OnClientPresenceChangeStart += playerFactory.ClientPresenceChangeStartHandler;
            }
            while (levelData.Player == null)
                await UniTask.Yield();
            
            await loadStaticDataTask;
            staticData.NoiseSettings.seed = args.Seed;
            
            await terrainGenerator.GenerateTerrain();
            
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
        public NetworkConnection CurrentConnection;
        public int Seed;
    }
}