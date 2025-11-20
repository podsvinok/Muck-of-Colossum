using Cysharp.Threading.Tasks;
using FishNet.Connection;
using FishNet.Managing.Scened;
using UnityEngine;

namespace Code.Gameplay.Player.Factory
{
    public interface IPlayerFactory
    {
        public UniTask<GameObject> SpawnPlayer(NetworkConnection connection);
        public void ClientPresenceChangeStartHandler(ClientPresenceChangeEventArgs args);
    }
}