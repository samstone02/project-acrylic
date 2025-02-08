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

        public int MagazineCount { get => MagazineCountNetVar.Value; }

        private NetworkVariable<bool> _isReloading = new NetworkVariable<bool>(false);

        private NetworkVariable<bool> _isInterClipReloading = new NetworkVariable<bool>(false);
        
        private float _interClipReloadTimer;
        
        private NetworkVariable<int> MagazineCountNetVar { get; set; } = new NetworkVariable<int>(0);

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
            if (_isReloading.Value)
            {
                if (IsServer)
                {
                    ReloadTimer.Value -= Time.deltaTime;
                    ShellLoadTimer.Value -= Time.deltaTime;
                }

                if (ReloadTimer.Value <= 0)
                {
                    if (IsServer)
                    {
                        MagazineCountNetVar.Value = Math.Min(CurrentAmmo, MagazineCapacity);
                        _isReloading.Value = false;
                    }
                    else
                    {
                        OnReloadEnd();
                    }
                }
                else if (ShellLoadTimer.Value <= 0)
                {
                    if (IsServer)
                    {
                        ShellLoadTimer.Value = ReloadTimeSeconds / MagazineCapacity;
                    }
                    else
                    {
                        ShellLoadEvent?.Invoke();
                    }
                }
            }
            else if (_isInterClipReloading.Value)
            {
                if (IsServer)
                {
                    _interClipReloadTimer -= Time.deltaTime;
                }

                if (_interClipReloadTimer <= 0)
                {
                    if (IsServer)
                    {
                        _isInterClipReloading.Value = false;
                    }
                    else
                    {
                        // TODO: I think the interclip reload isn't firing BC it is never <= 0 on the client.
                        // Solution is to have a separate client and server timer?
                        InterClipReloadEndEvent?.Invoke();
                    }
                }
            }
        }

        public override void Fire()
        {
            if (_isReloading.Value || _isInterClipReloading.Value || MagazineCountNetVar.Value <= 0)
            {
                return; 
            }

            base.Fire();

            FireRpc();
        }

        public override void Reload()
        {
            if (_isReloading.Value || MagazineCountNetVar.Value == MagazineCapacity)
            {
                return;
            }

            OnReloadStart();
            ReloadRpc();
        }

        [Rpc(SendTo.Server)]
        private void FireRpc()
        {
            if (_isReloading.Value || MagazineCountNetVar.Value == 0)
            {
                return;
            }

            var shell = ProjectilePrefab;
            MagazineCountNetVar.Value--;

            if (MagazineCountNetVar.Value == 0)
            {
                if (shell.GetComponent<Shell>().EmpoweredProjectile != null)
                {
                    shell = shell.GetComponent<Shell>().EmpoweredProjectile;
                }
            }

            GameObject projectile = LaunchProjectile(shell);

            if (MagazineCountNetVar.Value == 0)
            {
                Reload();
            }
            else
            {
                _isInterClipReloading.Value = true;
                _interClipReloadTimer = InterClipReloadTimeSeconds;
            }
        }

        [Rpc(SendTo.Server)]
        private void ReloadRpc()
        {
            if (CurrentAmmo > 0)
            {
                MagazineCountNetVar.Value = 0;
                _isReloading.Value = true;
                ReloadTimer.Value = ReloadTimeSeconds;
                ShellLoadTimer.Value = ReloadTimeSeconds / MagazineCapacity;
            }
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