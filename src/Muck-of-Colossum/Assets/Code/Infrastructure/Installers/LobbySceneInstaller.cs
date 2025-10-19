using Code.Gameplay.Lobby;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.Installers
{
    /// <summary>
    /// Scene-specific installer for the Lobby scene.
    /// Binds lobby-specific components and ensures proper injection.
    /// </summary>
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