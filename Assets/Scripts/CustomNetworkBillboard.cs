using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CustomNetworkBillboard : NetworkBehaviour
{
    private Transform _cameraTrans;

    // TODO: This (player nametags) don't work anymore. Fix that.

    void Start()
    {
        if (IsClient)
        {
            this.enabled = false;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientId, sceneName, mode) =>
            {
                if (sceneName == "Lab")
                {
                    this.enabled = true;
                    _cameraTrans = FindFirstObjectByType<Camera>().transform;
                }
            };
        }
    }

    private void LateUpdate()
    {
        if (IsClient)
        {
            transform.rotation = _cameraTrans.rotation;
        }
    }
}
