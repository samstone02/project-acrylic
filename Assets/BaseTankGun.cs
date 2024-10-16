using UnityEngine;

public abstract class BaseTankGun : MonoBehaviour
{
    [field: SerializeField]
    public GameObject ProjectilePrefab { get; set; }

    protected Transform BulletSpawnPoint;

    private void Awake()
    {
        BulletSpawnPoint = transform.Find("BulletSpawnPoint");
    }
    
    public abstract GameObject Shoot();
}
