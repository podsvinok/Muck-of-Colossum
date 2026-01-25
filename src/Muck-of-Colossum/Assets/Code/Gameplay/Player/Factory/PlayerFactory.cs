using Code.Gameplay.Levels;
using Code.Infrastructure.AssetManagement;
using Code.Infrastructure.StaticData;
using Code.Random;
using Code.Utils;
using Cysharp.Threading.Tasks;
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
        private readonly IStaticDataService staticData;
        private readonly IRandomService random;

        public PlayerFactory(
            IAssetProvider assets,
            ILevelDataProvider levelData,
            NetworkManager networkManager,
            IStaticDataService staticData,
            IRandomService random)
        {
            this.assets = assets;
            this.levelData = levelData;
            this.networkManager = networkManager;
            this.staticData = staticData;
            this.random = random;
        }

        public async UniTask<GameObject> SpawnPlayerAtRandomPoint(NetworkConnection connection)
        {
            Vector3 spawnPosition = await GetRandomSpawnPosition();
            return await SpawnPlayer(connection, spawnPosition);
        }

        public async UniTask<GameObject> SpawnPlayer(NetworkConnection connection)
        {
            Vector3 spawnPosition = GetSpawnPosition();
            return await SpawnPlayer(connection, spawnPosition);
        }

        private async UniTask<GameObject> SpawnPlayer(NetworkConnection connection, Vector3 position)
        {
            var playerObject = await CreatePlayer(position);
            var networkObject = playerObject.GetComponent<NetworkObject>();
            
            networkManager.ServerManager.Spawn(networkObject, connection);

            return playerObject;
        }

        private async UniTask<GameObject> CreatePlayer(Vector3 at)
        {
            var playerPrefab = await assets.LoadAsync(AssetPath.PlayerPath);
            var newPlayer = Object.Instantiate(playerPrefab, at, Quaternion.identity);

            return newPlayer;
        }

        private async UniTask<Vector3> GetRandomSpawnPosition()
        {
            var rayStart = new Vector3(
                random.GetRandomFloatInRange(-staticData.MeshSettings.meshWorldSize / 2, staticData.MeshSettings.meshWorldSize / 2),
                staticData.HeightMapSettings.heightMultiplier * 1.1f,
                random.GetRandomFloatInRange(-staticData.MeshSettings.meshWorldSize / 2, staticData.MeshSettings.meshWorldSize / 2));

            RaycastHit hit;
            while (!Physics.Raycast(rayStart, Vector3.down, out hit, staticData.HeightMapSettings.heightMultiplier * 1.1f, 1 << LayerMask.NameToLayer(Layers.Ground)))
                await UniTask.Yield();
            //Physics.Raycast(rayStart, Vector3.down, out hit, staticData.HeightMapSettings.heightMultiplier * 1.1f, 1 << LayerMask.NameToLayer(Layers.Ground));
            return new Vector3(hit.point.x, hit.point.y + 2, hit.point.z);
        }

        private Vector3 GetSpawnPosition()
        {
            var rayStart = new Vector3(
                levelData.StartPoint.x,
                staticData.HeightMapSettings.heightMultiplier * 1.1f,
                levelData.StartPoint.z);
            
            if (!Physics.Raycast(rayStart, Vector3.down, out var hit, staticData.HeightMapSettings.heightMultiplier * 1.1f))
                Debug.LogError($"there's no spot under {rayStart}");
            
            return new Vector3 (hit.point.x, hit.point.y + 2, hit.point.z);
        }
    }
}