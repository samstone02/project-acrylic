using System;
using System.Collections.Generic;
using System.Linq;
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

        private bool _isReloading;
        
        private bool _isInterClipReloading;

        private float _reloadTimer;
        
        private float _interClipReloadTimer;
        
        private float _loadShellTimer;
        
        private List<GameObject> Magazine { get; set; } = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();
            StartReload();
        }

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

                if (_reloadTimer <= ReloadTimeSeconds - ReloadTimeSeconds / MagazineCapacity * (Magazine.Count + 1))
                {
                    Magazine.Add(NextShellToLoadPrefab);
                }
            }

            if (_isInterClipReloading)
            {
                _interClipReloadTimer -= Time.deltaTime;

                if (_interClipReloadTimer <= 0)
                {
                    _isInterClipReloading = false;
                }
            }
        }

        public override GameObject Fire()
        {
            if (_isReloading || _isInterClipReloading || Magazine.Count <= 0)
            {
                return null;
            }

            GameObject projectile = LaunchProjectile(Magazine.Last());
            Magazine.RemoveAt(Magazine.Count - 1);

            if (Magazine.Count == 0)
            {
                StartReload();
            }
            else
            {
                _isInterClipReloading = true;
                _interClipReloadTimer = InterClipReloadTimeSeconds;
            }

            return projectile;
        }

        public override void StartReload()
        {
            if (_isReloading || Magazine.Count == MagazineCapacity)
            {
                return;
            }

            Magazine.Clear();
            _isReloading = true;
            _reloadTimer = ReloadTimeSeconds;
        }
    }   
}