using Unity.Netcode;
using UnityEngine;

public class NetworkingControls : MonoBehaviour
{
    private NetworkManager _networkManager;
    private SessionOrchestrator _gameManager;
    
    void Start()
    {
        _networkManager = GetComponent<NetworkManager>();
        _gameManager = FindAnyObjectByType<SessionOrchestrator>();
    }

    protected void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if (_networkManager.IsConnectedClient)
        {
            RenderConnectedGui();
        }
        else 
        {
            RenderDisconnectedGui();
        }

        GUILayout.EndArea();
    }

    private void RenderConnectedGui()
    {
        if (_networkManager.IsHost)
        {
            RenderHostGui();
        }
        else if (_networkManager.IsServer)
        {
            RenderServerGui();
        }
        else if (_networkManager.IsClient)
        {
            RenderClientGui();
        }

        if (GUILayout.Button("Leave Session"))
        {
            _gameManager.LeaveSession();
        }
    }

    private void RenderDisconnectedGui()
    {
        if (GUILayout.Button("Start Host"))
        {
            _networkManager.StartHost();
        }
        
        if (GUILayout.Button("Start Server"))
        {
            _networkManager.StartServer();
        }
        
        if (GUILayout.Button("Start Client"))
        {
            _networkManager.StartClient();
        }
    }
    
    private void RenderHostGui()
    {
        GUILayout.Label("Host");
    }

    private void RenderServerGui()
    {
        GUILayout.Label("Server");
    }
    
    private void RenderClientGui()
    {
        GUILayout.Label("Client");
    }
}
