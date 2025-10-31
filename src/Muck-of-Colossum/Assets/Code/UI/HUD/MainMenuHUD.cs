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
        
        private IGameStateMachine stateMachine;

        [Inject]
        public void Construct(IGameStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        private void Awake() => 
            EnterLobbyButton.onClick.AddListener(EnterLobbyLoadingState);

        private void OnDestroy() => 
            EnterLobbyButton.onClick.RemoveListener(EnterLobbyLoadingState);

        private void EnterLobbyLoadingState() => 
            stateMachine.Enter<LobbyLoadingState>();
    }
}