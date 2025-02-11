using Unity.Netcode;
using UnityEngine;

public class NetworkingControls : MonoBehaviour
{
    private NetworkManager _networkManager;
    
    void Start()
    {
        _networkManager = GetComponent<NetworkManager>();
    }

    protected void OnGUI()
    {
        if (!_networkManager.IsServer && !_networkManager.IsClient)
        {
            RenderDisconnectedGui();
        }
        else if (_networkManager.IsHost)
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
    }

    private void RenderDisconnectedGui()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

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
        
        GUILayout.EndArea();
    }
    
    private void RenderHostGui()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        GUILayout.Label("Host");
        
        GUILayout.EndArea();
    }

    private void RenderServerGui()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        GUILayout.Label("Server");
        
        GUILayout.EndArea();
    }
    
    private void RenderClientGui()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        GUILayout.Label("Client");
        
        GUILayout.EndArea();
    }
}
