using Code.Gameplay.Levels;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using Zenject;

namespace Code.Network
{
    public class GameplayLoadingStateRPCs : NetworkBehaviour
    {
        private ILevelDataProvider levelData;
        
        [Inject]
        public void Construct(ILevelDataProvider levelData)
        {
            this.levelData = levelData;
        }
        
        [TargetRpc]
        public void UpdateClientPlayerData(NetworkConnection connection, GameObject playerObject)
        {
            levelData.Player = playerObject.transform;
        }
    }
}