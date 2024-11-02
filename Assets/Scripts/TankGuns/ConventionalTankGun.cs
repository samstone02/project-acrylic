using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankGuns
{
    public class ConventionalTankGun : BaseTankGun
    {
        [field: SerializeField] public float ReloadTimeSeconds { get; set; } = 5;
        
        public override event Action OnReloadEnd;

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
                    OnReloadEnd?.Invoke();
                }
            }
        }

        public override GameObject Fire()
        {
            if (_isReloading)
            {
                return null;
            }

            GameObject projectile = LaunchProjectile();

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

        private GameObject LaunchProjectile()
        {
            var projectile = Instantiate(ProjectilePrefab);
            projectile.transform.position = ShellSpawnPoint.position;
            projectile.transform.rotation = ShellSpawnPoint.rotation;
            var rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = projectile.transform.forward * 20;
            return projectile;
        }
    }   
}