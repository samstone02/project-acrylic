using Unity.Netcode;
using UnityEngine;

public class CustomNetworkBillboard : NetworkBehaviour
{
    private Transform _cameraTrans;

    void Start()
    {
        if (IsClient)
        {
            _cameraTrans = FindFirstObjectByType<Camera>().transform;
        }
    }

    void LateUpdate()
    {
        if (IsClient)
        {
            transform.rotation = _cameraTrans.rotation;
        }
    }
}
