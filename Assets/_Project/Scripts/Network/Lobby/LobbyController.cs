using System;
using System.Collections.Generic;
using _Project.Scripts.Network.Lobby;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;


    public class LobbyController : NetworkBehaviour
    {
        [SerializeField] private MainLobbyPanel _mainLobbyScreen;
        [SerializeField] private CreateLobbyScreen _createScreen;
        [SerializeField] private RoomPanel _roomScreen;
        //[SerializeField] private PlayerHUD _playerScreen;
        //[SerializeField] private Camera _lobbyUiCamera;
        
        private void Start() 
        {
            _mainLobbyScreen.OpenPopup();
            _createScreen.ClosePopup();
            _roomScreen.ClosePopup();

            CreateLobbyScreen.LobbyCreated += CreateLobby;
            LobbyRoomPanel.LobbySelected += OnLobbySelected;
            RoomPanel.LobbyLeft += OnLobbyLeft;
            RoomPanel.StartPressed += OnGameStart;
        
            NetworkObject.DestroyWithScene = true;
        }
        
        #region Main Lobby

        private async void OnLobbySelected(Lobby lobby) 
        {
            try 
            { 
                await Matchmaking.JoinLobbyWithAllocation(lobby.Id);

                _mainLobbyScreen.ClosePopup();
                _roomScreen.OpenPopup();

                NetworkManager.Singleton.StartClient();
            }
            catch (Exception e) {
                Debug.LogError("Failed joining lobby" + e);
            }
        }
        
        #endregion
        
        #region Create

        private async void CreateLobby(LobbyData data) 
        {
            try 
            {
                await Matchmaking.CreateLobbyWithAllocation(data);

                _createScreen.ClosePopup();
                _roomScreen.OpenPopup();

                // Starting the host immediately will keep the relay server alive
                NetworkManager.Singleton.StartHost();
            }
            catch (Exception e) 
            {
                Debug.LogError(e);
            }
        }
        
        #endregion
        
        #region Room
        
        private readonly Dictionary<ulong, bool> _playersInLobby = new(); 
        public static event Action<Dictionary<ulong, bool>> LobbyPlayersUpdated; 
        private float _nextLobbyUpdate;
        
        public override void OnNetworkSpawn() 
        { 
            if (IsServer) {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
                _playersInLobby.Add(NetworkManager.Singleton.LocalClientId, false);
                UpdateInterface();
            }

            // Client uses this in case host destroys the lobby
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;

        }
        
        /// <summary>
        /// Client connection to lobby.
        /// </summary>
        /// <param name="playerId"></param>
        private void OnClientConnectedCallback(ulong playerId) 
        { 
            if (!IsServer) return;

            // Add locally
             if (!_playersInLobby.ContainsKey(playerId)) _playersInLobby.Add(playerId, false);
             
             PropagateToClients();
             
             UpdateInterface(); 
        }
        
        /// <summary>
        /// For each player in lobby screen sends rpc from client to update ready or other statuses.
        /// </summary>
        private void PropagateToClients() 
        { 
            foreach (var player in _playersInLobby) UpdatePlayerClientRpc(player.Key, player.Value); 
        }
        
        /// <summary>
        /// Gets client ids from players updates their ready status
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="isReady"></param>
        [ClientRpc] 
        private void UpdatePlayerClientRpc(ulong clientId, bool isReady) 
        {
            if (IsServer) return;

            if (!_playersInLobby.ContainsKey(clientId)) _playersInLobby.Add(clientId, isReady);
            else _playersInLobby[clientId] = isReady; 
            UpdateInterface(); 
        }
        private void OnClientDisconnectCallback(ulong playerId) 
        {
            if (IsServer) 
            {
                // Handle locally
                if (_playersInLobby.ContainsKey(playerId)) _playersInLobby.Remove(playerId);

                // Propagate all clients
                RemovePlayerClientRpc(playerId);

                UpdateInterface();
            }
            else {
                // This happens when the host disconnects the lobby
                _roomScreen.ClosePopup();
                _mainLobbyScreen.OpenPopup();
                OnLobbyLeft();
            }
        }

        /// <summary>
        /// Removes disconnected player sends rpc from client.
        /// </summary>
        /// <param name="clientId"></param>
        [ClientRpc]
        private void RemovePlayerClientRpc(ulong clientId) 
        {
            if (IsServer) return;

            if (_playersInLobby.ContainsKey(clientId)) _playersInLobby.Remove(clientId);
            UpdateInterface();
        }

        public void OnReadyClicked() 
        {
            SetReadyServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        /// <summary>
        /// Update UI. Inform players that you are ready.
        /// </summary>
        /// <returns></returns>
        [ServerRpc(RequireOwnership = false)]
        private void SetReadyServerRpc(ulong playerId) 
        {
            _playersInLobby[playerId] = true;
            PropagateToClients();
            UpdateInterface();
        }

     
        private void UpdateInterface() 
        {
            LobbyPlayersUpdated?.Invoke(_playersInLobby);
        }
    
        #endregion
    
        private async void OnLobbyLeft() 
        {
            _playersInLobby.Clear();
            NetworkManager.Singleton.Shutdown();
            await Matchmaking.LeaveLobby();
        }
    
        public override void OnDestroy() 
        {
            base.OnDestroy();
            CreateLobbyScreen.LobbyCreated -= CreateLobby;
            LobbyRoomPanel.LobbySelected -= OnLobbySelected;
            RoomPanel.LobbyLeft -= OnLobbyLeft;
            RoomPanel.StartPressed -= OnGameStart;
        
            // We only care about this during lobby
            if (NetworkManager.Singleton != null) {
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            }
      
        }
        
        /// <summary>
        /// Starts game loads the game scene (optional)
        /// </summary>
        private async void OnGameStart() 
        {
            await Matchmaking.LockLobby();

            _mainLobbyScreen.ClosePopup();
            _createScreen.ClosePopup();
            _roomScreen.ClosePopup();
            
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);

            //lobbyUiCamera.enabled = false;

            //_playerScreen.OpenScreen();
        }
    } 

