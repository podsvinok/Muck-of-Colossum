using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Code.Infrastructure.SceneManagement;
using Code.Infrastructure.States.StateMachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Infrastructure.States.GameStates
{
    public class BootstrapState : IState
    {
        private readonly IGameStateMachine gameStateMachine;
        private readonly ISceneLoader sceneLoader;
        
        public BootstrapState(IGameStateMachine gameStateMachine, ISceneLoader sceneLoader)
        {
            this.gameStateMachine = gameStateMachine;
            this.sceneLoader = sceneLoader;
        }

        public UniTask Enter()
        {
            gameStateMachine.Enter<GameEnterState>();
            return UniTask.CompletedTask;
        }

        public async UniTask Exit()
        {
            await sceneLoader.LoadScene("Game");
        }
    }
}