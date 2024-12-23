using Unity.Netcode;
using UnityEngine;

namespace Projectiles
{
    public class Shell : NetworkBehaviour
    {
        [field: SerializeField] public GameObject EmpoweredProjectile { get; set; } = null;

        [field: SerializeField] public int Damage { get; set; } = 1;
        
        [field: SerializeField] public float Speed { get; set; } = 25.0f;

        [field: SerializeField] public bool IgnoreArmor { get; set; } = false;
        
        protected void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }
    }
}