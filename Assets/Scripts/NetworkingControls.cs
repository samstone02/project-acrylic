using Unity.Netcode;
using UnityEngine;

public class NetworkingControls : MonoBehaviour
{
    private NetworkManager networkManager;
    
    void Start()
    {
        networkManager = GetComponent<NetworkManager>();
    }

    protected void OnGUI()
    {
        if (!networkManager.IsServer && !networkManager.IsClient)
        {
            RenderDisconnectedGui();
        }
        else if (networkManager.IsHost)
        {
            RenderHostGui();
        }
        else if (networkManager.IsServer)
        {
            RenderServerGui();
        }
        else if (networkManager.IsClient)
        {
            RenderClientGui();
        }
    }

    private void RenderDisconnectedGui()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));

        if (GUILayout.Button("Start Host"))
        {
            networkManager.StartHost();
        }
        
        if (GUILayout.Button("Start Server"))
        {
            networkManager.StartServer();
        }
        
        if (GUILayout.Button("Start Client"))
        {
            networkManager.StartClient();
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
