using _Project.Scripts.Car;
using _Project.Scripts.Managers;
using _Project.Scripts.UI;
using Controllers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Network
{
   public class PlayerNetwork : NetworkBehaviour
   {
       // Network variables with write permissions based on server or client authority.
    private readonly NetworkVariable<Vector3> _netPosition = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<Quaternion> _netRotation = new NetworkVariable<Quaternion>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<PlayerNetworkData> _netState;
    
    // Components and game objects used by the player.
    private ArcadeVehicleController _playerInput;
    private CameraController _cameraController;
    private Rigidbody _rb;
    [SerializeField] private PlayerHUD _playerScreen;
    [SerializeField] private GameObject _sphere;
    [SerializeField] private GameObject _visual;
    private Transform[] _spawnPositions;
    
    private Vector3 _vel;
    private float _rotVel;
    private bool _serverAuth;

    private void OnEnable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManagerOnLoadComplete;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= SceneManagerOnLoadComplete;
    }
    
    private void Awake()
    {
        _playerInput = GetComponent<ArcadeVehicleController>();
        _cameraController = GetComponent<CameraController>();
        _rb = GetComponent<Rigidbody>();
        var permission = _serverAuth ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner;
        _netState = new NetworkVariable<PlayerNetworkData>(writePerm: permission);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _playerInput.enabled = IsOwner;
        _cameraController.playerCamera.enabled = IsOwner;
    }

    private void SceneManagerOnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (sceneName != "Game") return;
        _playerScreen = Services.Instance.UIManager.playerHUD;
        _spawnPositions = Services.Instance.GameFlowManager.positions;
        _rb.isKinematic = false;
        MoveToRandomTransform();
        ActivatePlayerScreen();
        ActivateGameObjects();
    }

    private void MoveToRandomTransform()
    {
        int randomIndex = Random.Range(0, _spawnPositions.Length);
        Transform randomTransform = _spawnPositions[randomIndex];
        transform.position = randomTransform.position;
        transform.rotation = randomTransform.rotation;
    }

    private void ActivatePlayerScreen()
    {
        _playerScreen.OpenScreen();
    }

    private void ActivateGameObjects()
    {
        _sphere.SetActive(true);
        _visual.SetActive(true);
    }

    private void Update()
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

    [ServerRpc]
    private void TransmitStateServerRPC(PlayerNetworkData state)
    {
        _netState.Value = state;
    }

    private void ConsumeState()
    {
        var position = Vector3.SmoothDamp(transform.position, _netState.Value.Position, ref _vel, 0.1f);
        var rotation = Quaternion.Euler(_netState.Value.Rotation);
        transform.SetPositionAndRotation(position, rotation);
    }
   }
}
