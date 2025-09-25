using Code.Gameplay.Player.Movement;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Inputs;
using Code.Utils;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Code.Gameplay.Player.Factory
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IAssetProvider assets;
        private readonly IInputService input;
        
        public PlayerFactory(IAssetProvider assets, IInputService input)
        {
            this.assets = assets;
            this.input = input;
        }

        public async UniTask<GameObject> CreatePlayer(Vector3 at)
        {
            var playerPrefab = await assets.LoadAsset(AssetPath.PlayerPath);
            var newPlayer = Object.Instantiate(playerPrefab, at, Quaternion.identity);
            newPlayer.GetComponent<PlayerMove>().Construct(input);
            return newPlayer;
        }

        public async UniTask<NetworkObject> CreatePlayerNetwork(Vector3 at, ulong id)
        {
            var player = await CreatePlayer(at);
            var networkObj = player.GetComponent<NetworkObject>();
            networkObj.SpawnAsPlayerObject(id);
            return networkObj;
        }
    }
}