using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Code.Gameplay.Player.Factory
{
    public interface IPlayerFactory
    {
        public UniTask<GameObject> CreatePlayer(Vector3 at);
        public UniTask<NetworkObject> CreatePlayerNetwork(Vector3 at, ulong id);
    }
}