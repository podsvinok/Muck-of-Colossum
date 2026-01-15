using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.UI.HUD
{
    public class MainMenuHUD : MonoBehaviour
    {
        [SerializeField] private Button EnterLobbyButton;
        [SerializeField] private Button ExitButton;
        
        private IGameStateMachine stateMachine;

        [Inject]
        public void Construct(IGameStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        private void Awake()
        {
            EnterLobbyButton.onClick.AddListener(EnterLobbyLoadingState);
            ExitButton.onClick.AddListener(EnterExitGameState);
        }

        private void OnDestroy()
        {
            EnterLobbyButton.onClick.AddListener(EnterLobbyLoadingState);
            ExitButton.onClick.RemoveListener(EnterExitGameState);
        }

        private void EnterLobbyLoadingState() => 
            stateMachine.Enter<LobbyLoadingState>();
        
        private void EnterExitGameState() => 
            stateMachine.Enter<ExitGameState>();
    }
}