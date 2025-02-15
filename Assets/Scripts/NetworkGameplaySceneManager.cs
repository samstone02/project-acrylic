using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameplaySceneManager : NetworkBehaviour
{
    [field: SerializeField] public TeamManager TeamManagerPrefab { get; private set; }

    [field: SerializeField] public string GameplayOverlaySceneName { get; private set; }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // InstantiateAndSpawn was giving me a hard time for some reason
            var tm = Instantiate(TeamManagerPrefab);
            tm.NetworkObject.Spawn();
            DontDestroyOnLoad(tm.gameObject);

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