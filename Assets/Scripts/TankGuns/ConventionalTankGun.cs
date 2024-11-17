using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankGuns
{
    public class ConventionalTankGun : BaseTankGun
    {
        [field: SerializeField] public float ReloadTimeSeconds { get; set; } = 5;
        
        public override event Action OnMagazineReloadEnd;

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
                    OnMagazineReloadEnd?.Invoke();
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

            StartReload();

            return projectile;
        }

        public override void StartReload()
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