using TMPro;
using UnityEngine;

namespace _Project.Scripts.Network.Lobby
{
    /// <summary>
    /// Used for the prefab in room panel. Controls player name and ready status
    /// </summary>
    public class LobbyPlayerPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText, _statusText;

        public ulong PlayerId { get; private set; }

        public void Init(ulong playerId) {
            PlayerId = playerId;
            _nameText.text = $"Player {playerId}";
        }

        public void SetReady() {
            _statusText.text = "Ready";
            _statusText.color = Color.green;
        }
    }
}