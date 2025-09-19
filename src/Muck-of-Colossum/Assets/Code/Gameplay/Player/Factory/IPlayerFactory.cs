using UnityEngine;

namespace Code.Gameplay.Player.Factory
{
    public interface IPlayerFactory
    {
        public void CreatePlayer(Vector3 at);
    }
}