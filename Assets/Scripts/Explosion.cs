using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Explosion : MonoBehaviour
    {
        private int _damage = 0;
        
        protected void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
            {
                var tank = collision.gameObject.GetComponent<Tank>() ?? throw new Exception("Expected vehicle to have a Tank component");
                tank.TakeDamage(_damage);
            }
        }
        
        public void Explode(float duration, int damage)
        {
            Invoke(nameof(EndExplode), duration);
            _damage = damage;
        }

        private void EndExplode()
        {
            Destroy(gameObject);
        }
    }
}