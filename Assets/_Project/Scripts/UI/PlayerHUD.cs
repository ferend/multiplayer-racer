using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class PlayerHUD : UIScreen
    {
        [SerializeField] private GameObject _movementJoystick; 
        protected override void OnScreenOpen()
        {
           
        }

        protected override void OnScreenClose()
        {
        }

        public void ExitGameButton()
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}