using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing;
using UnityEngine;

namespace Code.Gameplay.Player.Factory
{
    public interface IPlayerFactory
    {
        public UniTask SpawnPlayer(NetworkConnection connection);
    }
}