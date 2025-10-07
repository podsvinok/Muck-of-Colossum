using System;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using FishNet.Managing;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.UI.HUD
{
    public class LobbyHUD : MonoBehaviour
    {
        [SerializeField] private Button startHostButton;
        [SerializeField] private Button startClientButton;
        
        private IGameStateMachine stateMachine;
        private NetworkManager networkManager;
        
        [Inject]
        public void Construct(IGameStateMachine stateMachine, NetworkManager networkManager)
        {
            this.stateMachine = stateMachine;
            this.networkManager = networkManager;
        }

        private void Awake()
        {
            startHostButton.onClick.AddListener(EnterGameplayLoadingStateAsHost);
            startClientButton.onClick.AddListener(EnterGameplayLoadingStateAsClient);
        }

        private void EnterGameplayLoadingStateAsClient()
        {
            networkManager.ClientManager.StartConnection();
            
            EnterGameplayLoadingState();
        }

        private void EnterGameplayLoadingStateAsHost()
        {
            networkManager.ServerManager.StartConnection();
            networkManager.ClientManager.StartConnection();
            
            EnterGameplayLoadingState();
        }

        private void EnterGameplayLoadingState() =>
            stateMachine.Enter<GameplayLoadingState>();

        private void OnDestroy()
        {
            startHostButton.onClick.RemoveListener(EnterGameplayLoadingStateAsHost);
            startClientButton.onClick.RemoveListener(EnterGameplayLoadingStateAsClient);
        }
    }
}