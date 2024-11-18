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
        
        public event Action ShellLoad;
        
        public event Action InterClipReloadEnd;
        
        public float ReloadTimer { get; private set; }

        private bool _isReloading;
        
        private bool _isInterClipReloading;
        
        private float _interClipReloadTimer;
        
        private float _loadShellTimer;
        
        private List<GameObject> Magazine { get; set; } = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();
            Reload();
        }

        private void Update()
        {
            if (_isReloading)
            {
                ReloadTimer -= Time.deltaTime;
                
                if (ReloadTimer <= 0)
                {
                    _isReloading = false;
                    OnReloadEnd();
                }

                if (ReloadTimer <= ReloadTimeSeconds - ReloadTimeSeconds / MagazineCapacity * (Magazine.Count + 1))
                {
                    Magazine.Add(NextShellToLoadPrefab);
                    ShellLoad?.Invoke();
                }
            }

            if (_isInterClipReloading)
            {
                _interClipReloadTimer -= Time.deltaTime;

                if (_interClipReloadTimer <= 0)
                {
                    _isInterClipReloading = false;
                    InterClipReloadEnd?.Invoke();
                }
            }
        }

        public override GameObject Fire()
        {
            if (_isReloading || _isInterClipReloading || Magazine.Count <= 0)
            {
                return null;
            }
            
            base.OnFire();

            GameObject projectile = LaunchProjectile(Magazine.Last());
            Magazine.RemoveAt(Magazine.Count - 1);

            if (Magazine.Count == 0)
            {
                Reload();
            }
            else
            {
                _isInterClipReloading = true;
                _interClipReloadTimer = InterClipReloadTimeSeconds;
            }

            return projectile;
        }

        public override void Reload()
        {
            if (_isReloading || Magazine.Count == MagazineCapacity)
            {
                return;
            }

            OnReloadStart();
            Magazine.Clear();
            _isReloading = true;
            ReloadTimer = ReloadTimeSeconds;
        }
    }   
}