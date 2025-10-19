using System;
using Code.Gameplay.Player.Factory;
using Code.Utils;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;
using Zenject;

namespace Code.Network
{
    /// <summary>
    /// Handles network lifecycle events and player spawning across different scenes
    /// </summary>
    public class NetworkSceneLoader : IInitializable, IDisposable
    {
        private readonly IPlayerFactory playerFactory;
        private readonly NetworkManager networkManager;
        
        [Inject]
        public NetworkSceneLoader(
            IPlayerFactory playerFactory,
            NetworkManager networkManager)
        {
            this.playerFactory = playerFactory;
            this.networkManager = networkManager;
        }

        public void Initialize()
        {
            networkManager.SceneManager.OnLoadEnd += OnSceneLoadEnd;
        }

        /// <summary>
        /// Called when a scene finishes loading
        /// </summary>
        private void OnSceneLoadEnd(SceneLoadEndEventArgs args)
        {
            if (!networkManager.IsServerStarted)
                return;

            string sceneName = args.LoadedScenes[0].name;

            if (sceneName == AssetPath.LobbyScene)
            {
                // Spawn players in lobby
                SpawnLobbyPlayers(args).Forget();
            }
            
            else if (sceneName == AssetPath.GameScene)
            {
                // Spawn players in game
                SpawnGamePlayers(args).Forget();
            }
        }

        /// <summary>
        /// Spawn lobby players (simplified versions or full players marked as lobby)
        /// </summary>
        private async UniTask SpawnLobbyPlayers(SceneLoadEndEventArgs args)
        {
            foreach (NetworkConnection conn in args.QueueData.Connections)
            {
                // You could spawn a simplified lobby representation
                // or use the same player prefab but at a lobby spawn point
                await playerFactory.SpawnPlayer(conn);
            }
        }

        /// <summary>
        /// Spawn game players at game-specific spawn points
        /// </summary>
        private async UniTask SpawnGamePlayers(SceneLoadEndEventArgs args)
        {
            foreach (NetworkConnection conn in args.QueueData.Connections)
            {
                // Despawn existing player if exists
                if (conn.FirstObject != null)
                {
                    networkManager.ServerManager.Despawn(conn.FirstObject);
                }

                // Spawn new player at game spawn point
                await playerFactory.SpawnPlayer(conn);
            }
        }

        public void Dispose()
        {
            networkManager.SceneManager.OnLoadEnd -= OnSceneLoadEnd;
        }
    }
}