using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Car;
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
        

        private void Awake()
        {
            _playerInput = this.GetComponent<ArcadeVehicleController>();
            // If server authority is false we are the client authority.
            var permission = _serverAuth ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner;
            
            _netState = new NetworkVariable<PlayerNetworkData>(writePerm: permission);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner)
            {
                // junk code will fix later
            }

            if (!IsOwner)
            {
                _playerInput.enabled = false;
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
                TransmitStateServerRPC(state);        // We are telling server to this is our new state can you porpagate it rest of the clients.

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

struct PlayerNetworkData : INetworkSerializable
{
    private float _x, _z, _y;
    private short _xRot, _yRot, _zRot; // changed values to short for network optimization

    internal Vector3 Position
    {
        get => new Vector3(_x, _y, _z);
        set
        {
            _x = value.x;
            _z = value.z;
            _y = value.y;
        }
    }

    internal Vector3 Rotation
    {
        get => new Vector3(_xRot, _yRot, _zRot);
        set
        {
            _xRot = (short) value.x;
            _yRot = (short) value.y;
            _zRot = (short) value.z;
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _x);
        serializer.SerializeValue(ref _z);
        serializer.SerializeValue(ref _y);
        serializer.SerializeValue(ref _yRot);
        serializer.SerializeValue(ref _xRot);
        serializer.SerializeValue(ref _zRot);
    }
}