using System.Text.RegularExpressions;
using Code.Network;
using Code.Network.Lobby;
using FishNet.Managing;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace Code.UI.HUD
{
    public class LobbyHUD : MonoBehaviour
    {
        [Header("Connection Buttons")]
        [SerializeField] private Button startHostButton;
        [SerializeField] private Button startClientButton;
        [SerializeField] private Button startGameButton;
        
        [Header("Lobby UI")]
        [SerializeField] private GameObject connectionPanel;
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI readyButtonText;
        [SerializeField] private Transform playerListContainer;
        [SerializeField] private GameObject playerListItemPrefab;
        [SerializeField] private TMP_InputField serverIP;
        
        private NetworkManager networkManager;
        private LobbyService lobbyService;
        
        private bool isReady;

        [Inject]
        public void Construct(
            NetworkManager networkManager,
            LobbyService lobbyService)
        {
            this.networkManager = networkManager;
            this.lobbyService = lobbyService;
        }

        private void Awake()
        {
            startHostButton.onClick.AddListener(OnStartAsHostButtonClick);
            startClientButton.onClick.AddListener(OnStartAsClientButtonClick);
            startGameButton.onClick.AddListener(OnStartGameButtonClick);
            readyButton.onClick.AddListener(OnReadyButtonClick);
            
            lobbyService.OnLobbyPlayerChanged += OnPlayerListChanged;
            
            ShowConnectionPanel();
        }

        private void OnStartAsClientButtonClick() => 
            StartAsClient();

        private void OnStartAsHostButtonClick() => 
            StartAsHost();

        private void OnReadyButtonClick() => 
            ToggleReady();

        private void OnStartGameButtonClick() => 
            lobbyService.CheckAllReady();

        private void OnPlayerListChanged() => 
            RefreshPlayerList();

        private void ToggleReady()
        {
            isReady = !isReady;
            
            lobbyService.SetPlayerReady(networkManager.ClientManager.Connection, isReady);
            
            UpdateReadyButton();
        }

        private void ShowConnectionPanel()
        {
            lobbyPanel.SetActive(false);
            connectionPanel.SetActive(true);
        }

        private void ShowLobbyPanel()
        {
            connectionPanel.SetActive(false);
            lobbyPanel.SetActive(true);
            
            RefreshPlayerList();
        }

        private void UpdateReadyButton()
        {
            if (isReady)
            {
                readyButtonText.text = "Ready";
                readyButton.GetComponent<Image>().color = Color.green;
            }
            else
            {
                readyButtonText.text = "Not Ready";
                readyButton.GetComponent<Image>().color = Color.white;
            }
        }

        private void StartAsHost()
        {
            networkManager.ServerManager.StartConnection();
            networkManager.ClientManager.StartConnection();
            
            ShowLobbyPanel();
        }

        private void StartAsClient()
        { 
            if (Regex.IsMatch(serverIP.text, @"^(((?!25?[6-9])[12]\d|[1-9])?\d\.?\b){4}$"))
                networkManager.TransportManager.Transport.SetClientAddress(serverIP.text);
            networkManager.ClientManager.StartConnection();
            
            ShowLobbyPanel();
        }

        private void RefreshPlayerList()
        {
            foreach (Transform child in playerListContainer)
                Destroy(child.gameObject);

            foreach (var player in lobbyService.GetPlayers())
            {
                var item = Instantiate(playerListItemPrefab, playerListContainer);
                var itemText = item.GetComponentInChildren<TextMeshProUGUI>();
                
                string readyStatus = player.Value.IsReady ? "[READY]" : "[NOT READY]";
                itemText.text = $"{player.Value.PlayerName} {readyStatus}";
                itemText.color = player.Value.IsReady ? Color.green : Color.white;
            }
        }

        private void OnDisable()
        {
            startHostButton.onClick.RemoveListener(OnStartAsHostButtonClick);
            startClientButton.onClick.RemoveListener(OnStartAsClientButtonClick);
            startGameButton.onClick.RemoveListener(OnStartGameButtonClick);
            readyButton.onClick.RemoveListener(OnReadyButtonClick);
            
            lobbyService.OnLobbyPlayerChanged -= OnPlayerListChanged;
        }
    }
}