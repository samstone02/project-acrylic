using Unity.Netcode;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private GameObject _target;

    private Vector3 _offset;

    public void Start()
    {
        _target = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
        _offset = transform.position - _target.transform.position;
        this.transform.parent = null;
    }
    
    private void Update()
    {        
        transform.position = _target.transform.position + _offset;
    }
}
