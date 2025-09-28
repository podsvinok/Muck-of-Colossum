using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Cysharp.Threading.Tasks;
using FishNet.Managing;
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

        public UniTask Enter()
        {
            return UniTask.CompletedTask;
        }

        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }
}