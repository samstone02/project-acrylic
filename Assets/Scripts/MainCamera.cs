using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public GameObject target;

    private Vector3 _offset;

    private void Awake()
    {
       _offset = transform.position - target.transform.position;
    }
    
    private void Update()
    {
        transform.position = target.transform.position + _offset;
    }
}
