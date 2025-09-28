using System;
using Code.Gameplay.Player.Factory;
using Code.Utils;
using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;
using Zenject;

namespace Code.Network
{
    public class NetworkExtensions : NetworkBehaviour
    {
        private IPlayerFactory playerFactory;
        
        private Action<NetworkConnection, bool> spawnPlayerAction;

        [Inject]
        public void Construct(IPlayerFactory playerFactory)
        {
            this.playerFactory = playerFactory;
        }

        public override void OnStartServer()
        {
            Debug.Log("NetworkExtensions.OnStartServer");
            //TODO: spawn on start
            
            //spawnPlayerAction = UniTaskHelper.Action<NetworkConnection, bool>(OnClientLoadedStartScenes);
            //SceneManager.OnClientLoadedStartScenes += spawnPlayerAction;
        }

        public override void OnStopServer()
        {
            Debug.Log("NetworkExtensions.OnStopServer");
            //SceneManager.OnClientLoadedStartScenes -= spawnPlayerAction;
            //spawnPlayerAction = null;
        }

        public override async void OnSpawnServer(NetworkConnection connection)
        {
            Debug.Log("NetworkExtensions.OnSpawnServer");
            if (connection.LoadedStartScenes(true))
                await SpawnPlayer(connection);
        }

        private async UniTask OnClientLoadedStartScenes(NetworkConnection connection, bool asServer)
        {
            Debug.Log("NetworkExtensions.OnClientLoadedStartScenes");
            if (asServer && Observers.Contains(connection))
                await SpawnPlayer(connection);
        }

        private async UniTask SpawnPlayer(NetworkConnection connection)
        {
            Debug.Log("NetworkExtensions.SpawnPlayer");
            await playerFactory.SpawnPlayer(connection);
        }
    }
}