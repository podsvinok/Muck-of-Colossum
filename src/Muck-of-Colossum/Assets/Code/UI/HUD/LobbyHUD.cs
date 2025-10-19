using System;
using System.Collections.Generic;
using Code.Gameplay.Lobby;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Code.UI.HUD
{
    /// <summary>
    /// Lobby UI that displays player list and handles ready states
    /// </summary>
    public class LobbyHUD : MonoBehaviour
    {
        [Header("Connection Buttons")]
        [SerializeField] private Button startHostButton;
        [SerializeField] private Button startClientButton;
        
        [Header("Lobby UI")]
        [SerializeField] private GameObject connectionPanel;
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI readyButtonText;
        [SerializeField] private Transform playerListContainer;
        [SerializeField] private GameObject playerListItemPrefab;
        
        private IGameStateMachine stateMachine;
        private NetworkManager networkManager;
        private LobbyService lobbyService;
        
        private bool isReady;

        [Inject]
        public void Construct(IGameStateMachine stateMachine, NetworkManager networkManager, LobbyService lobbyService)
        {
            this.stateMachine = stateMachine;
            this.networkManager = networkManager;
            this.lobbyService = lobbyService;
        }

        private void Awake()
        {
            startHostButton.onClick.AddListener(StartAsHost);
            startClientButton.onClick.AddListener(StartAsClient);
            readyButton.onClick.AddListener(ToggleReady);
            
            lobbyService.OnLobbyPlayerChanged += OnPlayerListChanged;
            
            ShowConnectionPanel();
        }

        private void StartAsHost()
        {
            networkManager.ServerManager.StartConnection();
            networkManager.ClientManager.StartConnection();
            
            ShowLobbyPanel();
        }

        private void StartAsClient()
        {
            networkManager.ClientManager.StartConnection();
            
            ShowLobbyPanel();
        }

        private void ToggleReady()
        {
            if (lobbyService == null || !networkManager.IsClientStarted)
                return;

            isReady = !isReady;
            
            // Send ready state to server
            lobbyService.SetPlayerReady(networkManager.ClientManager.Connection, isReady);
            
            UpdateReadyButton();
        }

        private void UpdateReadyButton()
        {
            readyButtonText.text = isReady ? "Not Ready" : "Ready";
            readyButton.GetComponent<Image>().color = isReady ? Color.white : Color.green;
        }

        private void OnPlayerListChanged()
        {
            RefreshPlayerList();
        }

        /// <summary>
        /// Refresh player list UI
        /// </summary>
        private void RefreshPlayerList()
        {
            // Clear existing list
            foreach (Transform child in playerListContainer)
                Destroy(child.gameObject);

            // Create new list items
            foreach (var kvp in lobbyService.GetPlayers())
            {
                var item = Instantiate(playerListItemPrefab, playerListContainer);
                var itemText = item.GetComponentInChildren<TextMeshProUGUI>();
                
                string readyStatus = kvp.Value.IsReady ? "[READY]" : "[NOT READY]";
                itemText.text = $"{kvp.Value.PlayerName} {readyStatus}";
                itemText.color = kvp.Value.IsReady ? Color.green : Color.white;
            }
        }

        private void ShowConnectionPanel()
        {
            connectionPanel.SetActive(true);
            lobbyPanel.SetActive(false);
        }

        private void ShowLobbyPanel()
        {
            connectionPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            RefreshPlayerList();
        }

        private void OnDisable()
        {
            startHostButton.onClick.RemoveListener(StartAsHost);
            startClientButton.onClick.RemoveListener(StartAsClient);
            readyButton.onClick.RemoveListener(ToggleReady);
            
            lobbyService.OnLobbyPlayerChanged -= OnPlayerListChanged;
        }
    }
}