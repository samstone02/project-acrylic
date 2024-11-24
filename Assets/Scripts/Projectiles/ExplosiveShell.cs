using DefaultNamespace;
using UnityEngine;

namespace Projectiles
{
    public class ExplosiveShell : Shell
    {
        [field: SerializeField] public GameObject ExplosionPrefab { get; set; }
        
        [field: SerializeField] public float ExplosionDuration { get; set; }
        
        [field: SerializeField] public int ExplosionDamage { get; set; }
        
        protected new void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
            
            Debug.Log("Explode!");

            var explosion = Instantiate(ExplosionPrefab).GetComponent<Explosion>();
            explosion.Explode(ExplosionDuration, ExplosionDamage);
            explosion.transform.position = transform.position;
        }
    }
}