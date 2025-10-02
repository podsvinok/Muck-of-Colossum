using System;
using Code.Gameplay.Player.Factory;
using Code.Utils;
using Cysharp.Threading.Tasks;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;
using Zenject;

namespace Code.Network
{
    public class NetworkExtensions : IInitializable, IDisposable
    {
        private readonly IPlayerFactory playerFactory;
        private readonly NetworkManager networkManager;
        
        private Action<NetworkConnection, bool> spawnPlayerAction;

        [Inject]
        public NetworkExtensions(IPlayerFactory playerFactory, NetworkManager networkManager)
        {
            this.playerFactory = playerFactory;
            this.networkManager = networkManager;
        }

        public void Initialize()
        {
            networkManager.SceneManager.OnClientLoadedStartScenes += spawnPlayerAction;
            spawnPlayerAction = UniTaskHelper.Action<NetworkConnection, bool>(OnClientLoadedStartScenes);
        }

        public void Dispose()
        {
            networkManager.SceneManager.OnClientLoadedStartScenes -= spawnPlayerAction;
        }
        
        private async UniTask OnClientLoadedStartScenes(NetworkConnection connection, bool asServer)
        {
            if (asServer) 
                await SpawnPlayer(connection);
        }

        private async UniTask SpawnPlayer(NetworkConnection connection)
        {
            await playerFactory.SpawnPlayer(connection);
        }
    }
}