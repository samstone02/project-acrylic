using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public GameObject target;

    private Vector3 _offset;

    private void Awake()
    {
        if (target is null)
        {
            return;
        }
        
        _offset = transform.position - target.transform.position;
    }
    
    private void Update()
    {
        if (target is null)
        {
            return;
        }
        
        transform.position = target.transform.position + _offset;
    }
}
