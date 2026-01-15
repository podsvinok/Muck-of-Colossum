using Code.Network.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.HUD
{
    public class LobbyPlayerItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text name;
        [SerializeField] private TMP_Text readyText;
        [SerializeField] private Image readyImage;

        public void Initialize(LobbyPlayer player)
        {
            SetName(player);
            SetReady(player);
        }

        private void SetName(LobbyPlayer player)
        {
            readyText.text = player.IsReady ? "Ready" : "Not Ready";
            name.text = player.PlayerName;
        }

        private void SetReady(LobbyPlayer player) => 
            readyImage.color = player.IsReady ? Color.green : Color.white;
    }
}