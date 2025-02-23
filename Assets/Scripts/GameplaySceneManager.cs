using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplaySceneManager : NetworkBehaviour
{
    [field: SerializeField] public string GameplayOverlaySceneName { get; private set; }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SceneManager.LoadScene("Lobby", LoadSceneMode.Additive);
        }

        if (IsClient)
        {
            NetworkManager.SceneManager.OnLoadComplete += (clientId, name, mode) =>
            {
                if (name == "Lab")
                {
                    LoadGameplayOverlay();
                }
            };
        }
    }

    public void LeaveGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGameplayScene()
    {
        UnloadLobbySceneClientRpc();
        NetworkManager.SceneManager.LoadScene("Lab", LoadSceneMode.Additive);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void UnloadLobbySceneClientRpc()
    {
        SceneManager.UnloadSceneAsync("Lobby");
    }

    public void LoadGameplayOverlay()
    {
        SceneManager.LoadSceneAsync("GameplayOverlay", LoadSceneMode.Additive);
    }
}