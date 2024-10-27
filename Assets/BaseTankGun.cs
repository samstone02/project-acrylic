using UnityEngine;

public abstract class BaseTankGun : MonoBehaviour
{
    [field: SerializeField]
    public GameObject ProjectilePrefab { get; set; }

    protected Transform BulletSpawnPoint;

    protected virtual void Awake()
    {
        BulletSpawnPoint = transform.Find("BulletSpawnPoint");
    }
    
    public abstract GameObject Fire();
    
    public abstract void Reload();
}
