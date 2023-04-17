using Unity.Netcode;
using UnityEngine;

namespace _Project.Scripts.Network
{
 
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
}