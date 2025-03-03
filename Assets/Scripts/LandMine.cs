using DefaultNamespace;
using Projectiles;
using Unity.Netcode;
using UnityEngine;

public class LandMine : NetworkBehaviour
{
    [field: SerializeField] public float Damage { get; private set; }

    [field: SerializeField] public float DurationSeconds { get; private set; }

    [field: SerializeField] public NetworkObject ExplosionPrefab { get; private set; }

    private void Start()
    {
        if (IsClient)
        {
            return;
        }
    }

    /* Explosions triggering with LandMines are handled in the Explision MonoBehaviour */

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<Tank>() != null || col.gameObject.GetComponent<Shell>() != null)
        {
            this.Explode();
        }

        if (col.gameObject.GetComponent<Shell>() != null)
        {
            Destroy(col.gameObject);
        }
    }

    public void Explode()
    {
        var exp = NetworkManager.SpawnManager.InstantiateAndSpawn(ExplosionPrefab, position: this.transform.position);
        var exp2 = exp.GetComponent<Explosion>();
        exp2.Explode(DurationSeconds, Damage);
        Destroy(this.gameObject);
    }
}
