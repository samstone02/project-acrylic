using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplaySceneManager : NetworkBehaviour
{
    [field: SerializeField] public string GameplayOverlaySceneName { get; private set; }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene("Lab", LoadSceneMode.Additive);

            NetworkManager.OnClientConnectedCallback += (clientId) =>
            {
                if (clientId == OwnerClientId)
                {
                    SceneManager.LoadScene(GameplayOverlaySceneName, LoadSceneMode.Additive);
                }
            };
        }
    }

    public void LeaveGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}