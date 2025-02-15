using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameplaySceneManager : NetworkBehaviour
{
    [field: SerializeField] public string GameplayOverlaySceneName { get; private set; }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.SceneManager.LoadScene("Lab", LoadSceneMode.Additive);

            NetworkManager.OnClientConnectedCallback += (_) =>
            {
                if (IsClient)
                { 
                    SceneManager.LoadScene(GameplayOverlaySceneName, LoadSceneMode.Additive);
                }
            };
        }
    }
}