using FishNet.Connection;

namespace Code.Gameplay.Lobby
{
    public struct LobbyPlayer
    {
        public NetworkConnection Connection;
        public bool IsReady;
        public bool IsServer;
        public string PlayerName;
    }
}