using Code.Network;
using Code.Network.Lobby;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    public class LobbySceneInstaller : MonoInstaller
    {
        [SerializeField] private LobbyService lobbyService;

        public override void InstallBindings()
        {
            BindLobbyService();
        }

        private void BindLobbyService()
        {
            Container
                .BindInterfacesAndSelfTo<LobbyService>()
                .FromInstance(lobbyService)
                .AsSingle();
        }
    }
}