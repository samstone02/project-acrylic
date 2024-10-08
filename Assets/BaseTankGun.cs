using UnityEngine;

public abstract class BaseTankGun : MonoBehaviour
{
    [field: SerializeField]
    public GameObject ProjectilePrefab { get; set; }
    
    public abstract GameObject Shoot();
}
