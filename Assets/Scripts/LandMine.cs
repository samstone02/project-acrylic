using DefaultNamespace;
using Projectiles;
using Unity.Netcode;
using UnityEngine;

public class LandMine : NetworkBehaviour
{
    [field: SerializeField] public float Damage { get; private set; }

    [field: SerializeField] public float DurationSeconds { get; private set; }

    [field: SerializeField] public Explosion ExplosionPrefab { get; private set; }

    private Explosion _explosion;

    private void Start()
    {
        if (IsClient)
        {
            return;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Tank>() != null || other.GetComponent<Shell>() != null)
        {
            this.Explode();
        }
    }

    private void Explode()
    {
        var exp = NetworkManager.SpawnManager.InstantiateAndSpawn(ExplosionPrefab.NetworkObject, position: this.transform.position);
        var exp2 = exp.GetComponent<Explosion>();
        exp2.Explode(DurationSeconds, Damage);
        Destroy(this.gameObject);
    }
}
