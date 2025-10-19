using System;
using System.Collections.Generic;
using System.Linq;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using Code.Utils;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Lobby
{
    /// <summary>
    /// Server-authoritative lobby manager that tracks player ready states
    /// and handles transition to game scene when all players are ready.
    /// </summary>
    public class 
        LobbyService : NetworkBehaviour
    {
        private readonly SyncDictionary<NetworkConnection, LobbyPlayer> players = new();
        
        private NetworkManager networkManager;
        private IGameStateMachine stateMachine;

        public event Action OnLobbyPlayerChanged;
        
        [Inject]
        public void Construct(NetworkManager networkManager, IGameStateMachine stateMachine)
        {
            this.networkManager = networkManager;
            this.stateMachine = stateMachine;
        }

        public void Initialize()
        {
            networkManager.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
        }

        public void Dispose()
        {
            networkManager.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
        }
        
        private void OnRemoteConnectionState(NetworkConnection conn, FishNet.Transporting.RemoteConnectionStateArgs args)
        {
            if (!networkManager.IsServerStarted) return;

            // Add new player to lobby
            if (args.ConnectionState == FishNet.Transporting.RemoteConnectionState.Started)
            {
                players.Add(conn, new LobbyPlayer
                {
                    Connection = conn,
                    IsReady = false,
                    PlayerName = $"Player {conn.ClientId}"
                });
                OnLobbyPlayerChanged?.Invoke();
            }
            // Remove disconnected player
            else if (args.ConnectionState == FishNet.Transporting.RemoteConnectionState.Stopped)
            {
                players.Remove(conn);
                OnLobbyPlayerChanged?.Invoke();
                CheckAllReady();
            }
        }

        /// <summary>
        /// Called by clients to toggle their ready state
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void SetPlayerReady(NetworkConnection sender, bool isReady)
        {
            if (players.ContainsKey(sender))
            {
                var player = players[sender];
                player.IsReady = isReady;
                players[sender] = player;

                OnLobbyPlayerChanged?.Invoke();
                CheckAllReady();
            }
        }

        private void CheckAllReady()
        {
            if (players.Count == 0) return;

            bool allReady = players.Values.All(p => p.IsReady);

            if (allReady && players.Count > 0)
            {
                // All players ready - start game transition
                StartGameTransition();
            }
        }

        private void StartGameTransition()
        {
            var sceneLoadData = new FishNet.Managing.Scened.SceneLoadData(AssetPath.GameScene)
            {
                ReplaceScenes = FishNet.Managing.Scened.ReplaceOption.All,
            };

            networkManager.SceneManager.LoadConnectionScenes(sceneLoadData);
        }

        /// <summary>
        /// Get current lobby state for UI display
        /// </summary>
        public IReadOnlyDictionary<NetworkConnection, LobbyPlayer> GetPlayers() => players;
    }

    public struct LobbyPlayer
    {
        public NetworkConnection Connection;
        public bool IsReady;
        public string PlayerName;
    }
}