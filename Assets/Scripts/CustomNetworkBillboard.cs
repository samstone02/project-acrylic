using Unity.Netcode;
using UnityEngine;

public class CustomNetworkBillboard : NetworkBehaviour
{
    private Transform _cameraTrans;

    void Start()
    {
        if (IsClient)
        {
            this.enabled = false;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += (clientId, sceneName, mode) =>
            {
                if (sceneName == "Lab")
                {
                    _cameraTrans = FindFirstObjectByType<Camera>().transform;
                    this.enabled = true;
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
