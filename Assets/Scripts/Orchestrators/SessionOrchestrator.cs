using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using static FindMatchUiManager;

public class SessionOrchestrator : NetworkBehaviour
{
    private FindMatchUiManager FindMatchUiManager { get; set; }
    private SessionSceneManager _sessionSceneManager;
    private PlayerSessionData _sessionData;

    public void Awake()
    {
        _sessionSceneManager = FindAnyObjectByType<SessionSceneManager>();
        LoadFindMatchScene();
    }

    public void Start()
    {
        NetworkManager.OnClientConnectedCallback += (clientId) =>
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                UnloadFindMatchScene();
            }
        };
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.OnServerStarted += OnServerStarted;
        NetworkManager.OnClientConnectedCallback += OnClientConnected;
    }

    private void LoadFindMatchScene()
    {
        _sessionSceneManager.LoadFindMatchScene();
        SceneManager.sceneLoaded += FindMatchSceneLoaded;
    }

    private void UnloadFindMatchScene()
    {
        _sessionSceneManager.UnloadFindMatchScene();
    }

    private void FindMatchSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _sessionSceneManager.FindMatchSceneName)
        {
            FindMatchUiManager = FindAnyObjectByType<FindMatchUiManager>();
            FindMatchUiManager.HostSessionEvent += HostSession;
            FindMatchUiManager.JoinSessionEvent += JoinSession;
            SceneManager.sceneLoaded -= FindMatchSceneLoaded;
        }
    }

    private void HostSession(HostSessionData hostSessionData)
    {
        if (string.IsNullOrEmpty(hostSessionData.PlayerDisplayName.ToString()))
        {
            hostSessionData.PlayerDisplayName = "Host";
        }

        _sessionData = hostSessionData;

        NetworkManager.StartHost();
    }

    private void JoinSession(JoinSessionData joinSessionData)
    {
        if (string.IsNullOrEmpty(joinSessionData.Ipv4Address))
        {
            joinSessionData.Ipv4Address = "127.0.0.1";
        }
        if (joinSessionData.PortNumber == 0)
        {
            joinSessionData.PortNumber = 7777;
        }
        if (string.IsNullOrEmpty(joinSessionData.PlayerDisplayName.ToString()))
        {
            joinSessionData.PlayerDisplayName = "Client" + new Random().Next(1000);
        }

        _sessionData = joinSessionData;

        var unityTransport = NetworkManager.GetComponent<UnityTransport>();
        unityTransport.ConnectionData = new UnityTransport.ConnectionAddressData
        {
            Address = joinSessionData.Ipv4Address,
            Port = joinSessionData.PortNumber,
            ServerListenAddress = "0.0.0.0",
        };

        NetworkManager.StartClient();
    }

    private void OnServerStarted()
    {
        _sessionSceneManager.LoadLobbyScene();
    }

    private void OnClientConnected(ulong connectedClientId)
    {
        if (NetworkManager.LocalClientId == connectedClientId)
        {
            SetClientDisplayNameServerRpc(connectedClientId, _sessionData.PlayerDisplayName);
        }
    }

    public void BeginGame()
    {
        _sessionSceneManager.LoadGameplayScene();
    }

    public void LeaveSession()
    {
        NetworkManager.Shutdown();
        Destroy(NetworkManager.gameObject); // Destroy manually since NetworkManager lives in DDoL
        //EndSessionClientRpc();
    }

    [Rpc(SendTo.Server)]
    private void SetClientDisplayNameServerRpc(ulong clientId, FixedString32Bytes displayName)
    {
        var clientNametag = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponentInChildren<PlayerNametag>();
        clientNametag.PlayerName = displayName;
    }

    //[Rpc(SendTo.ClientsAndHost)]s
    //public void EndSessionClientRpc()
    //{
    //    _sessionSceneManager.LeaveGame();
    //}
}
