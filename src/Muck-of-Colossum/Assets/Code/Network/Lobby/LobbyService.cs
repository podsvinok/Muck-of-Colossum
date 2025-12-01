using System;
using System.Collections.Generic;
using System.Linq;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using Zenject;

namespace Code.Network
{
    public class LobbyService : NetworkBehaviour
    {
        [SerializeField] private int seed;
        public event Action OnLobbyPlayerChanged;
        
        private readonly SyncDictionary<NetworkConnection, LobbyPlayer> players = new();
        
        private NetworkManager networkManager;
        private IGameStateMachine stateMachine;
        
        [Inject]
        public void Construct(
            NetworkManager networkManager,
            IGameStateMachine stateMachine)
        {
            this.networkManager = networkManager;
            this.stateMachine = stateMachine;
        }

        public override void OnStartServer() => 
            networkManager.SceneManager.OnClientLoadedStartScenes += OnClientLoadedStartScenes;

        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerReady(NetworkConnection sender, bool isReady) //TODO: show in clients too
        {
            if (players.ContainsKey(sender))    
            {
                var player = players[sender];
                player.IsReady = isReady;
                players[sender] = player;

                OnLobbyPlayerChanged?.Invoke();
            }
        }

        [Server]
        public void CheckAllReady()
        {
            bool allReady = players.Values.All(p => p.IsReady);

            if (allReady) 
                StartGameTransition();
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
                
            OnLobbyPlayerChanged?.Invoke();
        }

        [ObserversRpc]
        private void StartGameTransition()
        {
            var args = new GameplayLoadingStateEnterArgs()
            {
                AsServer = IsServerInitialized,
                Players = new List<LobbyPlayer>(players.Values),
                CurrentConnection = LocalConnection,
                Seed = seed //TODO: make ability to enter it in lobby
            };
            
            stateMachine.Enter<GameplayLoadingState, GameplayLoadingStateEnterArgs>(args);
        }

        public IReadOnlyDictionary<NetworkConnection, LobbyPlayer> GetPlayers() => players;
        
        private void OnDisable() => 
            networkManager.SceneManager.OnClientLoadedStartScenes -= OnClientLoadedStartScenes;
    }
}