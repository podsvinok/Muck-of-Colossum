using System;
using Code.Gameplay.Levels;
using Code.Infrastructure.AssetManagement;
using Code.Utils;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Gameplay.Player.Factory
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IAssetProvider assets;
        private readonly ILevelDataProvider levelData;
        private readonly NetworkManager networkManager;

        public PlayerFactory(IAssetProvider assets, ILevelDataProvider levelData, NetworkManager networkManager)
        {
            this.assets = assets;
            this.levelData = levelData;
            this.networkManager = networkManager;
        }

        public async UniTask SpawnPlayer(NetworkConnection connection)
        {
            Vector3 spawnPosition = GetSpawnPosition();
            await SpawnPlayer(connection, spawnPosition);
        }

        private async UniTask SpawnPlayer(NetworkConnection connection, Vector3 position)
        {
            // Create player instance
            GameObject playerObject = await CreatePlayer(position);

            // Get NetworkObject component
            NetworkObject networkObject = playerObject.GetComponent<NetworkObject>();

            // Spawn player for specific connection
            networkManager.ServerManager.Spawn(networkObject, connection);
            
            Debug.Log($"Spawned player for connection {connection.ClientId} at {position}");
        }

        /// <summary>
        /// Create player GameObject instance
        /// </summary>
        private async UniTask<GameObject> CreatePlayer(Vector3 at)
        {
            {
                var playerPrefab = await assets.Load(AssetPath.PlayerPath);
                
                if (playerPrefab == null)
                {
                    Debug.LogError($"Failed to load player prefab at path: {AssetPath.PlayerPath}");
                    return null;
                }

                var newPlayer = Object.Instantiate(playerPrefab, at, Quaternion.identity);
                return newPlayer;
            }
        }

        private Vector3 GetSpawnPosition() => 
            levelData.StartPoint;
    }
}