using System;
using Projectiles;
using Unity.Netcode;
using UnityEngine;

namespace DefaultNamespace
{
    public class Explosion : NetworkBehaviour
    {
        private float _damage;
        
        protected void OnTriggerEnter(Collider collider)
        {
            // Handle damaging players
            if (collider.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
            {
                var tank = collider.gameObject.GetComponent<Tank>() ?? throw new Exception("Expected vehicle to have a Tank component");
                tank.TakeDamage(_damage);
            }

            // Handle triggering landmines
            var landMine = collider.GetComponent<LandMine>();
            if (landMine != null)
            {
                landMine.Explode();
            }
        }
        
        public void Explode(float duration, float damage)
        {
            Invoke(nameof(EndExplosion), duration);
            _damage = damage;
        }

        /// <summary>
        /// Disable the collider and let the particle/sound effects finish. Destory after 5 seconds.
        /// </summary>
        public void EndExplosion()
        {
            var collider = GetComponent<Collider>();
            collider.enabled = false;
            Destroy(gameObject, 5f);
        }
    }
}