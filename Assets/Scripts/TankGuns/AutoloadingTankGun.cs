using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace TankGuns
{
    public class AutoloadingTankGun : BaseTankGun
    {
        [field: SerializeField] public int MagazineCapacity { get; set; } = 5;
        
        [field: SerializeField] public float ReloadTimeSeconds { get; set; } = 5f;

        [field: SerializeField] public float InterClipReloadTimeSeconds { get; set; } = 1f;
        
        public override event Action OnReloadEnd;

        private int _shellsInMagazine;

        private bool _isReloading;

        private float _reloadTimer;

        protected override void Awake()
        {
            base.Awake();

            _shellsInMagazine = MagazineCapacity;
        }

        private void Update()
        {
            if (_isReloading)
            {
                _reloadTimer -= Time.deltaTime;   
                
                if (_reloadTimer <= 0)
                {
                    _shellsInMagazine = MagazineCapacity;
                    _isReloading = false;
                    OnReloadEnd?.Invoke();
                }
            }
        }

        public override GameObject Fire()
        {
            if (_isReloading || _shellsInMagazine <= 0)
            {
                return null;
            }

            GameObject projectile = LaunchProjectile();
            _shellsInMagazine--;

            ReloadIfMagazineEmpty();

            return projectile;
        }

        public override void Reload()
        {
            if (_isReloading || _shellsInMagazine == MagazineCapacity)
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

        private void ReloadIfMagazineEmpty()
        {
            if (_shellsInMagazine == 0)
            {
             Reload();
            }
        }
    }   
}