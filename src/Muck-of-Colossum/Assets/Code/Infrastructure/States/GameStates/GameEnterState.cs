using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.States.GameStates
{
    public class GameEnterState : IState
    {
        private readonly IPlayerFactory playerFactory;
        private readonly ILevelDataProvider levelData;

        public GameEnterState(IPlayerFactory playerFactory, ILevelDataProvider levelData)
        {
            this.playerFactory = playerFactory;
            this.levelData = levelData;
        }

        public async UniTask Enter()
        {
            Debug.Log("GameEnterState");
            playerFactory.CreatePlayer(levelData.StartPoint);
            await UniTask.Yield();
        }

        public async UniTask Exit()
        {
            await UniTask.Yield();
        }
    }
}