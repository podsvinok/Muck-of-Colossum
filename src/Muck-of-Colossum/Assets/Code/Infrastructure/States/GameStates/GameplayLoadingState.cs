using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Code.Gameplay.Item;
using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Code.Gameplay.TerrainGeneration.Generators;
using Code.Infrastructure.SceneManagement;
using Code.Infrastructure.States.StateMachine;
using Code.Infrastructure.StaticData;
using Code.Network;
using Code.UI.LoadingCurtain;
using Code.UI.Services.Factory;
using Code.Utils;
using Cysharp.Threading.Tasks;
using FishNet.Broadcast;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using UnityEngine;

namespace Code.Infrastructure.States.GameStates
{
    public class GameplayLoadingState : IPayloadState<GameplayLoadingStateEnterArgs>
    {
        private readonly ISceneLoader sceneLoader;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IGameStateMachine stateMachine;
        private readonly IStaticDataService staticData;
        private readonly TerrainGenerator terrainGenerator;
        private readonly ILevelDataProvider levelData;
        private readonly IUIFactory uiFactory;
        private readonly IPlayerFactory playerFactory;
        private readonly NetworkManager networkManager;
        private readonly ItemDatabase itemDatabase;

        public GameplayLoadingState(
            ISceneLoader sceneLoader,
            ILoadingCurtain loadingCurtain,
            IGameStateMachine stateMachine,
            IStaticDataService staticData,
            TerrainGenerator terrainGenerator, 
            ILevelDataProvider levelData, 
            IUIFactory uiFactory,
            NetworkManager networkManager,
            IPlayerFactory playerFactory,
            ItemDatabase itemDatabase)
        {
            this.sceneLoader = sceneLoader;
            this.loadingCurtain = loadingCurtain;
            this.stateMachine = stateMachine;
            this.staticData = staticData;
            this.terrainGenerator = terrainGenerator;
            this.levelData = levelData;
            this.uiFactory = uiFactory;
            this.networkManager = networkManager;
            this.playerFactory = playerFactory;
            this.itemDatabase = itemDatabase;
        }
        
        public async UniTask Enter(GameplayLoadingStateEnterArgs args)
        {
            loadingCurtain.Show();
            
            await LoadScene(args);
            await CreateUIRoot();
            await GenerateTerrain(args.Seed);
            LoadItemDataBase();
            await SpawnPlayers(args);
            InitializeChunks();
            
            await stateMachine.Enter<GameplayLoopState>();
            loadingCurtain.Hide();
        }

        private async UniTask LoadScene(GameplayLoadingStateEnterArgs args)
        {
            var utcs = new UniTaskCompletionSource();

            void OnLoadEnd(SceneLoadEndEventArgs loadArgs) => 
                utcs.TrySetResult();

            networkManager.SceneManager.OnLoadEnd += OnLoadEnd;
            
            if (args.AsServer)
                sceneLoader.LoadSceneNetwork(Scenes.GameScene);
            
            await utcs.Task;
        }

        private async UniTask CreateUIRoot() => 
            await uiFactory.CreateUIRoot();

        private async UniTask GenerateTerrain(int seed)
        {
            staticData.NoiseSettings.seed = seed;
            await terrainGenerator.GenerateTerrain();
            await terrainGenerator.InitializeChunks();
        }

        private void LoadItemDataBase() => 
            itemDatabase.LoadItems();

        private async UniTask SpawnPlayers(GameplayLoadingStateEnterArgs args)
        {
            if (args.AsServer)
            {
                foreach (var lobbyPlayer in args.Players) 
                    await playerFactory.SpawnPlayerAtRandomPoint(lobbyPlayer.Connection);
            }

            while (levelData.Player == null)
                await UniTask.Yield();
        }

        private void InitializeChunks() => 
            terrainGenerator.InitializeViewer(levelData.Player.transform);

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