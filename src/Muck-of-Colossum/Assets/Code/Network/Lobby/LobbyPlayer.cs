using FishNet.Connection;

namespace Code.Network.Lobby
{
    public struct LobbyPlayer
    {
        public NetworkConnection Connection;
        public bool IsReady;
        public bool IsServer;
        public string PlayerName;
    }
}