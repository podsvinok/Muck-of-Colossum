using Cysharp.Threading.Tasks;
using FishNet.Connection;

namespace Code.Gameplay.Player.Factory
{
    public interface IPlayerFactory
    {
        public UniTask SpawnPlayer(NetworkConnection connection);
    }
}