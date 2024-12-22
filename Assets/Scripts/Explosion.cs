using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Explosion : MonoBehaviour
    {
        private int _damage;
        
        protected void OnTriggerEnter(Collider collider)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
            {
                var tank = collider.gameObject.GetComponent<Tank>() ?? throw new Exception("Expected vehicle to have a Tank component");
                tank.TakeDamageRpc(_damage);
            }
        }
        
        public void Explode(float duration, int damage)
        {
            Invoke(nameof(EndExplosion), duration);
            _damage = damage;
        }

        public void EndExplosion()
        {
            Destroy(gameObject);
        }
    }
}