using System;
using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Unity.Netcode;
using UnityEngine;

namespace Code.Network
{
    public class NetworkEventsHandler : INetworkEventsHandler, IDisposable
    {
        private IPlayerFactory playerFactory;
        private ILevelDataProvider levelData;

        public NetworkEventsHandler(IPlayerFactory playerFactory, ILevelDataProvider levelData)
        {
            this.playerFactory = playerFactory;
            this.levelData = levelData;
        }

        private void OnClientConnected(ulong clientId)
        {
            if (NetworkManager.Singleton.IsServer)
                playerFactory.CreatePlayerNetwork(new Vector3(0, 0, 0), clientId);
            else 
                playerFactory.CreatePlayer(new Vector3(0, 0, 0));
        }

        public void CreateHost()
        {
            Debug.Log("NetworkEventsHandler.CreateHost");
            
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            
            NetworkManager.Singleton.StartHost();
        }
        
        public void CreateClient()
        {
            Debug.Log("NetworkEventsHandler.CreateClient");
            
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            
            NetworkManager.Singleton.StartClient();   
        }

        public void Dispose()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}
