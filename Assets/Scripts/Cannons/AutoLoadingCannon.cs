using System;
using System.Collections.Generic;
using System.Linq;
using Projectiles;
using Unity.Netcode;
using UnityEngine;

namespace TankGuns
{
    public class AutoLoadingCannon : BaseCannon
    {
        [field: SerializeField] public int MagazineCapacity { get; set; } = 5;
        
        [field: SerializeField] public float ReloadTimeSeconds { get; set; } = 5f;

        [field: SerializeField] public float InterClipReloadTimeSeconds { get; set; } = 1f;
        
        public event Action ShellLoadEvent;
        
        public event Action InterClipReloadEndEvent;
        
        public NetworkVariable<float> ReloadTimer { get; private set; } = new NetworkVariable<float>(0);

        public NetworkVariable<float> ShellLoadTimer { get; private set; } = new NetworkVariable<float>(0);

        private NetworkVariable<bool> _isReloading = new NetworkVariable<bool>(false);

        private NetworkVariable<bool> _isInterClipReloading = new NetworkVariable<bool>(false);
        
        private float _interClipReloadTimer;
        
        private List<GameObject> Magazine { get; set; } = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();
        }

        public override void OnNetworkSpawn()
        {
            Reload();

            if (IsClient)
            {
                _isReloading.OnValueChanged += HandleReloadingChange;
            }
        }

        private void Update()
        {
            if (!IsServer)
            {
                return;
            }

            if (_isReloading.Value)
            {
                ReloadTimer.Value -= Time.deltaTime;
                ShellLoadTimer.Value -= Time.deltaTime;

                if (ReloadTimer.Value <= 0)
                {
                    for (int i = 0; i < MagazineCapacity; i++)
                    {
                        Magazine.Add(ProjectilePrefab);
                    }
                    _isReloading.Value = false;
                    OnReloadEnd();
                }
                else if (ShellLoadTimer.Value <= 0)
                {
                    ShellLoadEvent?.Invoke();
                    ShellLoadTimer.Value = ReloadTimeSeconds / MagazineCapacity;
                }
            }
            else if (_isInterClipReloading.Value)
            {
                _interClipReloadTimer -= Time.deltaTime;

                if (_interClipReloadTimer <= 0)
                {
                    _isInterClipReloading.Value = false;
                    InterClipReloadEndEvent?.Invoke();
                }
            }
        }

        [Rpc(SendTo.Server)]
        public override void FireRpc()
        {
            if (_isReloading.Value || _isInterClipReloading.Value || Magazine.Count <= 0)
            {
                return; 
            }
            
            base.OnFire();

            var shell = Magazine.Last();
            Magazine.RemoveAt(Magazine.Count - 1);

            if (Magazine.Count == 0)
            {
                if (shell.GetComponent<Shell>().EmpoweredProjectile != null)
                {
                    shell = shell.GetComponent<Shell>().EmpoweredProjectile;
                }
            }
            
            GameObject projectile = LaunchProjectile(shell);

            if (Magazine.Count == 0)
            {
                Reload();
            }
            else
            {
                _isInterClipReloading.Value = true;
                _interClipReloadTimer = InterClipReloadTimeSeconds;
            }
        }

        public override void Reload()
        {
            if (_isReloading.Value || Magazine.Count == MagazineCapacity)
            {
                return;
            }

            OnReloadStart();
            ReloadRpc();
        }

        [Rpc(SendTo.Server)]
        private void ReloadRpc()
        {
            Magazine.Clear();
            _isReloading.Value = true;
            ReloadTimer.Value = ReloadTimeSeconds;
            ShellLoadTimer.Value = ReloadTimeSeconds / MagazineCapacity;
        }

        private void HandleReloadingChange(bool previous, bool current)
        {
            if (current)
            {
                OnReloadStart();
            }
            else
            {
                OnReloadEnd();
            }
        }
    }   
}