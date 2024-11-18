using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankGuns
{
    public class ConventionalTankGun : BaseTankGun
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
                    OnReloadEnd();
                }
            }
        }

        public override GameObject Fire()
        {
            if (_isReloading)
            {
                return null;
            }

            GameObject projectile = LaunchProjectile(ProjectilePrefab);

            Reload();

            return projectile;
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