using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class PlayerHUD : UIScreen
    {
        [SerializeField] private GameObject _movementJoystick; 
        protected override void OnScreenOpen()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnScreenClose()
        {
            throw new System.NotImplementedException();
        }

        public void ExitGameButton()
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}