using System;
using Code.Gameplay.Levels;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Inputs;
using Code.Utils;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Gameplay.Player.Factory
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IAssetProvider assets;
        private readonly IInputService input;
        private readonly ILevelDataProvider levelData;
        
        public PlayerFactory(IAssetProvider assets, IInputService input, ILevelDataProvider levelData)
        {
            this.assets = assets;
            this.input = input;
            this.levelData = levelData;
        }

        public async UniTask SpawnPlayer(NetworkConnection connection)
        {
            var networkManager = InstanceFinder.NetworkManager;
            
            var player = await CreatePlayer(levelData.StartPoint);
            
            NetworkObject networkObject = networkManager.GetPooledInstantiated(player, true);
            
            networkManager.ServerManager.Spawn(networkObject, connection);
        }
        
        private async UniTask<GameObject> CreatePlayer(Vector3 at)
        {
            var playerPrefab = await assets.Load(AssetPath.PlayerPath);
            
            var newPlayer = Object.Instantiate(playerPrefab, at, Quaternion.identity);
            
            return newPlayer;
        }
    }
}