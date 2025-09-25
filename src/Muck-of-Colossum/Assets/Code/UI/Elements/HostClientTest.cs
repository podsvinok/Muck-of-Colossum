using System;
using Code.Network;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.UI.Elements
{
    public class HostClientTest : MonoBehaviour
    {
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;
        
        private INetworkEventsHandler networkHandler;

        [Inject]
        public void Construct(INetworkEventsHandler networkHandler)
        {
            this.networkHandler = networkHandler;
        }

        private void Awake()
        {
            hostButton.onClick.AddListener(networkHandler.CreateHost);
            clientButton.onClick.AddListener(networkHandler.CreateClient);
        }
        
        private void OnDestroy()
        {
            hostButton.onClick.RemoveListener(networkHandler.CreateHost);
            clientButton.onClick.RemoveListener(networkHandler.CreateClient);
        }
    }
}