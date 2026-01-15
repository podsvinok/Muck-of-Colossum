using System.Text.RegularExpressions;
using Code.Infrastructure.States.GameStates;
using Code.Infrastructure.States.StateMachine;
using Code.Network;
using Code.Network.Lobby;
using Cysharp.Threading.Tasks;
using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;
using Zenject;

namespace Code.UI.HUD
{
    public class LobbyHUD : MonoBehaviour
    {
        [SerializeField] private Button startHostButton;
        [SerializeField] private Button startClientButton;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private Button leaveLobbyButton;
        [SerializeField] private Button returnToMenuButton;
        
        [SerializeField] private GameObject connectionPanel;
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private GameObject playerListContainer;
        
        [SerializeField] private LobbyPlayerItem playerListItemPrefab;
        
        [SerializeField] private Transform playerList;
        [SerializeField] private TMP_InputField serverIP;
        
        private NetworkManager networkManager;
        private LobbyService lobbyService;
        private GameStateMachine stateMachine;
        
        private bool isReady;

        [Inject]
        public void Construct(
            NetworkManager networkManager,
            LobbyService lobbyService,
            GameStateMachine stateMachine)
        {
            this.networkManager = networkManager;
            this.lobbyService = lobbyService;
            this.stateMachine = stateMachine;
        }

        private void Awake()
        {
            startHostButton.onClick.AddListener(OnStartAsHostButtonClick);
            startClientButton.onClick.AddListener(OnStartAsClientButtonClick);
            startGameButton.onClick.AddListener(OnStartGameButtonClick);
            readyButton.onClick.AddListener(OnReadyButtonClick);
            leaveLobbyButton.onClick.AddListener(OnLeaveLobbyButton);
            returnToMenuButton.onClick.AddListener(OnReturnToMenuButton);
            
            lobbyService.OnLobbyPlayerChanged += OnPlayerListChanged;
            networkManager.ClientManager.OnClientConnectionState += OnServerStopped;
            
            ShowConnectionPanel();
        }


        private void OnServerStopped(ClientConnectionStateArgs args)
        {
            if (args.ConnectionState != LocalConnectionState.Stopped) return;
            
            isReady = false;
            ShowConnectionPanel();
        }

        private async void OnReturnToMenuButton() => 
            await ReturnToMainMenu();

        private void OnLeaveLobbyButton() => 
            LeaveLobby();

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
            connectionPanel.SetActive(true);
            
            lobbyPanel.SetActive(false);
            playerListContainer.SetActive(false);
        }

        private void ShowLobbyPanel(bool isHost)
        {
            lobbyPanel.SetActive(true);
            playerListContainer.SetActive(true);
            startGameButton.gameObject.SetActive(isHost);
            
            connectionPanel.SetActive(false);
            
            RefreshPlayerList();
            UpdateReadyButton();
        }

        private void UpdateReadyButton() => 
            readyButton.GetComponent<Image>().color = isReady ? Color.green : Color.white;

        private void StartAsHost()
        {
            networkManager.ServerManager.StartConnection();
            networkManager.ClientManager.StartConnection();
            
            ShowLobbyPanel(true);
        }

        private void LeaveLobby() => 
            lobbyService.LeaveLobby();

        private async UniTask ReturnToMainMenu() => 
            await stateMachine.Enter<MainMenuLoadingState>();

        private void StartAsClient()
        { 
            if (Regex.IsMatch(serverIP.text, @"^(((?!25?[6-9])[12]\d|[1-9])?\d\.?\b){4}$"))
                networkManager.TransportManager.Transport.SetClientAddress(serverIP.text);
            networkManager.ClientManager.StartConnection();
            
            ShowLobbyPanel(false);
        }

        private void RefreshPlayerList()
        {
            foreach (Transform child in playerList)
                Destroy(child.gameObject);

            foreach (var player in lobbyService.GetPlayers())
            {
                var item = Instantiate(playerListItemPrefab.gameObject, playerList);
                item.GetComponent<LobbyPlayerItem>().Initialize(player.Value);
            }
        }

        private void OnDisable()
        {
            startHostButton.onClick.RemoveListener(OnStartAsHostButtonClick);
            startClientButton.onClick.RemoveListener(OnStartAsClientButtonClick);
            startGameButton.onClick.RemoveListener(OnStartGameButtonClick);
            readyButton.onClick.RemoveListener(OnReadyButtonClick);
            leaveLobbyButton.onClick.RemoveListener(OnLeaveLobbyButton);
            returnToMenuButton.onClick.RemoveListener(OnReturnToMenuButton);
            
            lobbyService.OnLobbyPlayerChanged -= OnPlayerListChanged;
            networkManager.ClientManager.OnClientConnectionState -= OnServerStopped;
        }
    }
}