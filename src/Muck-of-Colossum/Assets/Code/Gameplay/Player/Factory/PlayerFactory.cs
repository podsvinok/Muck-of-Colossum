using Code.Gameplay.Levels;
using Code.Infrastructure.AssetManagement;
using Code.Utils;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
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

        public PlayerFactory(
            IAssetProvider assets,
            ILevelDataProvider levelData,
            NetworkManager networkManager)
        {
            this.assets = assets;
            this.levelData = levelData;
            this.networkManager = networkManager;
        }

        public async void ClientPresenceChangeStartHandler(ClientPresenceChangeEventArgs args)
        {
            var player = await SpawnPlayer(args.Connection);
            levelData.GameplayLoadingStateRPCs.UpdateClientPlayerData(args.Connection, player);
        }

        public async UniTask<GameObject> SpawnPlayer(NetworkConnection connection)
        {
            Vector3 spawnPosition = GetSpawnPosition();
            return await SpawnPlayer(connection, spawnPosition);
        }

        private async UniTask<GameObject> SpawnPlayer(NetworkConnection connection, Vector3 position)
        {
            GameObject playerObject = await CreatePlayer(position);

            NetworkObject networkObject = playerObject.GetComponent<NetworkObject>();

            networkManager.ServerManager.Spawn(networkObject, connection);

            return playerObject;
        }

        private async UniTask<GameObject> CreatePlayer(Vector3 at)
        {
            var playerPrefab = await assets.Load(AssetPath.PlayerPath);
            
            var newPlayer = Object.Instantiate(playerPrefab, at, Quaternion.identity);
            return newPlayer;
        }

        private Vector3 GetSpawnPosition() => 
            levelData.StartPoint;
    }
}