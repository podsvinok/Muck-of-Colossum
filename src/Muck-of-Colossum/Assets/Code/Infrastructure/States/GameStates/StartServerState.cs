using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.States.GameStates
{
    public class StartServerState : IState
    {
        private readonly IPlayerFactory playerFactory;
        private readonly ILevelDataProvider levelData;

        public StartServerState(IPlayerFactory playerFactory, ILevelDataProvider levelData)
        {
            this.playerFactory = playerFactory;
            this.levelData = levelData;
        }

        public async UniTask Enter()
        {
            await playerFactory.CreatePlayer(levelData.StartPoint);
        }

        public async UniTask Exit()
        {
            await UniTask.Yield();
        }
    }
}
