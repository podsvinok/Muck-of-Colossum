using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.Inputs;
using Code.Utils;
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

        public void CreatePlayer(Vector3 at)
        {
            var playerPrefab = assets.LoadAsset(AssetPath.PlayerPath);
            var newPlayer = Object.Instantiate(playerPrefab, at, Quaternion.identity);
            newPlayer.GetComponent<PlayerMove>().Construct(input);
        }
    }
}