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
using FishNet.Transporting;
using Zenject;

namespace Code.Network.Lobby
{
    public class LobbyService : NetworkBehaviour
    {
        public event Action OnLobbyPlayerChanged;
        
        private readonly SyncDictionary<int, LobbyPlayer> players = new();
        
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

        public override void OnStartServer()
        {
            base.OnStartServer();
            networkManager.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;
            networkManager.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            if (networkManager != null)
            {
                networkManager.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
                networkManager.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            players.OnChange += LobbyPlayerChange;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            players.OnChange -= LobbyPlayerChange;
        }
        
        private void OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
        {
            if (args.ConnectionState == RemoteConnectionState.Stopped)
            {
                if (players.ContainsKey(conn.ClientId)) 
                    players.Remove(conn.ClientId);
            }
        }

        private void OnClientLoadedStartScenes(NetworkConnection connection, bool asServer)
        {
            if (!asServer) return;
            
            players.Add(connection.ClientId, new LobbyPlayer
            {
                Connection = connection,
                IsReady = false,
                PlayerName = $"Player {connection.ClientId}",
                IsServer = connection.IsLocalClient
            });
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerReady(NetworkConnection sender, bool isReady)
        {
            if (players.ContainsKey(sender.ClientId))    
            {
                var player = players[sender.ClientId];
                player.IsReady = isReady;
                players[sender.ClientId] = player;
            }
        }

        [Server]
        public void CheckAllReady()
        {
            if (players.Count == 0) return;

            bool allReady = players.Values.All(p => p.IsReady);

            if (allReady)
            {
                var seed = random.GetRandomSeed();
                StartGameTransition(seed);
            }
        }

        private void LobbyPlayerChange(SyncDictionaryOperation op, int key, LobbyPlayer value, bool asServer) => 
            OnLobbyPlayerChanged?.Invoke();

        public void LeaveLobby()
        {
            if (IsServerInitialized)
                networkManager.ServerManager.StopConnection(false);
            else
                networkManager.ClientManager.StopConnection();
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

        public IReadOnlyDictionary<int, LobbyPlayer> GetPlayers() => players;
    }
}