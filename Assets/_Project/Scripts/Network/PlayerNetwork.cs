using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Car;
using _Project.Scripts.Network;
using Controllers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : NetworkBehaviour
    {
        // Client author. and server authr.
        private readonly NetworkVariable<Vector3> _netPosition = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Owner);
        private readonly NetworkVariable<Quaternion> _netRotation = new NetworkVariable<Quaternion>(writePerm: NetworkVariableWritePermission.Owner);
        private NetworkVariable<PlayerNetworkData> _netState;

        private bool _serverAuth;
        
        
        private Vector3 _vel;
        private float _rotVel;

        private ArcadeVehicleController _playerInput;
        private CameraController _cameraController;
        

        private void Awake()
        {
            _playerInput = this.GetComponent<ArcadeVehicleController>();
            _cameraController = this.GetComponent<CameraController>();
            // If server authority is false we are the client authority.
            var permission = _serverAuth ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner;
            
            _netState = new NetworkVariable<PlayerNetworkData>(writePerm: permission);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            this.gameObject.SetActive(true);
            if (IsOwner)
            {
                // junk code will fix later
            }

            if (!IsOwner)
            {
                _playerInput.enabled = false;
                _cameraController.playerCamera.enabled = false;
            }

        }

        /// <summary>
        /// TODO: removing works need to open lobby screens
        /// </summary>
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        void Update()
        {
            if (IsOwner)
            {
                TransmitState();
            }
            else
            {
                ConsumeState();
            }
            
        }

        private void TransmitState()
        {
            var state = new PlayerNetworkData()
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles
            };
            if (IsServer || !_serverAuth)
            {
                _netState.Value = state;
            }
            else
            {
                TransmitStateServerRPC(state);

            }
        }

        // RPC: Remote procedure call. If run on server update all clients. Can only run in server but triggered by clients.
        [ServerRpc]
        private void TransmitStateServerRPC(PlayerNetworkData state)
        {
            _netState.Value = state;
        }

        private void ConsumeState()
        {
            transform.position = Vector3.SmoothDamp(transform.position, _netState.Value.Position, ref _vel, 0.1f);
            transform.rotation = Quaternion.Euler(_netState.Value.Rotation.x, _netState.Value.Rotation.y,
                _netState.Value.Rotation.z);
        }
    }
