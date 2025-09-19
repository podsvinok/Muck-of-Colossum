using UnityEngine;

namespace Code.Infrastructure.Factories
{
    public interface IGameFactory
    {
        void CreatePlayer(Vector3 position);
    }
}