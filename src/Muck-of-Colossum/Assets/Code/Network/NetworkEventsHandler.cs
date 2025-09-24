using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Assets.Code.Network
{
    public class NetworkEventsHandler : NetworkBehaviour
    {
        private IPlayerFactory playerfactory;
        private ILevelDataProvider levelData;

        [Inject]
        public void Construct(IPlayerFactory playerFactory, ILevelDataProvider levelData)
        {
            this.playerfactory = playerFactory;
            this.levelData = levelData;
        }

        public void OnConnectedToServer()
        {
            Debug.Log("qwe");
            playerfactory.CreatePlayerNetwork(levelData.StartPoint, OwnerClientId);
        }
    }
}
