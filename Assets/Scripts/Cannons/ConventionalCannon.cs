using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankGuns
{
    public class ConventionalCannon : BaseCannon
    {
        [field: SerializeField] public float ReloadTimeSeconds { get; set; } = 5;

        private bool _isReloading;

        private float _reloadTimer;

        private void Update()
        {
            if (_isReloading)
            {
                _reloadTimer -= Time.deltaTime;   
                
                if (_reloadTimer <= 0)
                {
                    _isReloading = false;
                    InvokeReloadEnd();
                }
            }
        }

        public override void FireRpc()
        {
            if (_isReloading)
            {
                return;
            }

            base.OnFire();

            GameObject projectile = LaunchProjectile(ProjectilePrefab);

            Reload();
        }

        public override void Reload()
        {
            if (_isReloading)
            {
                return;
            }

            _isReloading = true;
            _reloadTimer = ReloadTimeSeconds;
        }
    }   
}