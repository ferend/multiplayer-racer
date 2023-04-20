using System;
using System.Threading.Tasks;
using _Project.Scripts.Managers;
using ParrelSync;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Network
{
    public class Authenticator : MonoBehaviour
    {
        public async void LoginAnonymously() {
            await Authentication.Login();
            SceneManager.LoadSceneAsync("_Project/Scenes/Lobby");
        }
    }
    
    public static class Authentication {
        public static string PlayerId { get; set; }

        public static async Task Login() {
            if (UnityServices.State == ServicesInitializationState.Uninitialized) {
                var options = new InitializationOptions();


#if UNITY_EDITOR
                // Remove this if you don't have ParrelSync installed. 
                // It's used to differentiate the clients, otherwise lobby will count them as the same
                if (ClonesManager.IsClone()) options.SetProfile(ClonesManager.GetArgument());
                else options.SetProfile("Primary");
#endif
            
                await UnityServices.InitializeAsync(options);
            }

            if (!AuthenticationService.Instance.IsSignedIn) {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                PlayerId = AuthenticationService.Instance.PlayerId;
            }
        }
    }
}
