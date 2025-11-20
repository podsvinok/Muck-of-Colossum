using FishNet.Connection;

namespace Code.Network
{
    public struct LobbyPlayer
    {
        public NetworkConnection Connection;
        public bool IsReady;
        public bool IsServer;
        public string PlayerName;
    }
}