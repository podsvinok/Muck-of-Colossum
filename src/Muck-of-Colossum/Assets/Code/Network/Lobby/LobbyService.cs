using System;
using System.Collections.Generic;
using System.Linq;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using Code.Random;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Zenject;

namespace Code.Network.Lobby
{
    public class LobbyService : NetworkBehaviour
    {
        public event Action OnLobbyPlayerChanged;
        
        private readonly SyncDictionary<NetworkConnection, LobbyPlayer> players = new();
        
        private NetworkManager networkManager;
        private IGameStateMachine stateMachine;
        private IRandomService random;

        [Inject]
        public void Construct(
            NetworkManager networkManager,
            IGameStateMachine stateMachine,
            IRandomService random)
        {
            this.networkManager = networkManager;
            this.stateMachine = stateMachine;
            this.random = random;
        }

        public override void OnStartServer() => 
            networkManager.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;

        public override void OnStartClient() => 
            players.OnChange += LobbyPlayerChange;

        private void LobbyPlayerChange(SyncDictionaryOperation op, NetworkConnection key, LobbyPlayer value, bool asServer) => 
            OnLobbyPlayerChanged?.Invoke();

        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerReady(NetworkConnection sender, bool isReady)
        {
            if (players.ContainsKey(sender))    
            {
                var player = players[sender];
                player.IsReady = isReady;
                players[sender] = player;
            }
        }

        [Server]
        public void CheckAllReady()
        {
            bool allReady = players.Values.All(p => p.IsReady);

            if (allReady)
            {
                var seed = random.GetRandomSeed();
                StartGameTransition(seed);
            }
        }

        private void OnClientLoadedStartScenes(NetworkConnection connection, bool asServer)
        {
            if (!asServer)
                return;
            
            players.Add(connection, new LobbyPlayer
            {
                Connection = connection,
                IsReady = false,
                PlayerName = $"Player {connection.ClientId}",
                IsServer = IsServerInitialized
            });
        }

        [ObserversRpc]
        private void StartGameTransition(int seed)
        {
            var args = new GameplayLoadingStateEnterArgs()
            {
                AsServer = IsServerInitialized,
                Players = new List<LobbyPlayer>(players.Values),
                CurrentConnection = LocalConnection,
                Seed = seed
            };
            
            stateMachine.Enter<GameplayLoadingState, GameplayLoadingStateEnterArgs>(args);
        }

        public IReadOnlyDictionary<NetworkConnection, LobbyPlayer> GetPlayers() => players;
        
        private void OnDisable() => 
            networkManager.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
    }
}