using Code.Gameplay.Levels;
using Code.Gameplay.Player.Factory;
using Code.Infrastructure.SceneManagement;
using Code.Infrastructure.States.StateMachine;
using Code.Utils;
using Cysharp.Threading.Tasks;
using FishNet.Managing;
using UnityEngine;

namespace Code.Infrastructure.States.GameStates
{
    public class GameLoadingState : IState
    {
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IGameStateMachine stateMachine;

        public GameLoadingState(IGameStateMachine stateMachine, ILoadingCurtain loadingCurtain)
        {
            this.loadingCurtain = loadingCurtain;
            this.stateMachine = stateMachine;
        }

        public async UniTask Enter()
        {
            loadingCurtain.Show();

            await stateMachine.Enter<MainMenuLoadingState>();
            
            loadingCurtain.Hide();
        }

        public UniTask Exit() => 
            default;
    }
}