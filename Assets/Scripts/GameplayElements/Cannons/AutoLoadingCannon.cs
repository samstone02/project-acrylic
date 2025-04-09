using System;
using System.Collections.Generic;
using System.Linq;
using Projectiles;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace TankGuns
{
    public class AutoLoadingCannon : BaseCannon
    {
        [field: SerializeField] public int MagazineCapacity { get; set; } = 5;
        
        [field: SerializeField] public float ReloadTimeSeconds { get; set; } = 5f;

        [field: SerializeField] public float FallbackReloadTimeSeconds { get; set; } = 2.5f;

        [field: SerializeField] public float InterClipReloadTimeSeconds { get; set; } = 1f;
        
        [field: SerializeField] public UnityEvent InterClipReloadEndEvent { get; private set; }

        public NetworkVariable<float> ReloadTimer { get; private set; } = new NetworkVariable<float>(0);

        public int MagazineCount { get => MagazineCountNetVar.Value; }

        private NetworkVariable<bool> _isReloading = new NetworkVariable<bool>(false);

        private NetworkVariable<bool> _isInterClipReloading = new NetworkVariable<bool>(false);
        
        private NetworkVariable<float> NetInterClipReloadTimer = new NetworkVariable<float>(0);
        
        private NetworkVariable<int> MagazineCountNetVar { get; } = new NetworkVariable<int>(0);

        protected override void Awake()
        {
            base.Awake();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

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
                }

                if (ReloadTimer.Value <= 0)
                {
                    if (IsServer)
                    {
                        MagazineCountNetVar.Value = Math.Min(AmmoReserve, MagazineCapacity);
                        _isReloading.Value = false;
                    }
                    else
                    {
                        base.OnReloadEnd();
                    }
                }
            }
            else if (_isInterClipReloading.Value)
            {
                if (IsServer)
                {
                    NetInterClipReloadTimer.Value -= Time.deltaTime;
                }

                if (NetInterClipReloadTimer.Value <= 0)
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
            if (_isReloading.Value || _isInterClipReloading.Value)
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
            if (_isReloading.Value)
            {
                return;
            }

            if (MagazineCountNetVar.Value <= 0 && AmmoReserve <= 0)
            {
                var fallbackShell = FallbackProjectilePrefab;
                GameObject fallbackProjectile = LaunchProjectile(fallbackShell);
                Reload();
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
                NetInterClipReloadTimer.Value = InterClipReloadTimeSeconds;
            }
        }

        [Rpc(SendTo.Server)]
        private void ReloadRpc()
        {
            if (MagazineCount < MagazineCapacity)
            {
                MagazineCountNetVar.Value = 0;
                _isReloading.Value = true;
                ReloadTimer.Value = AmmoReserve > 0 ? ReloadTimeSeconds : FallbackReloadTimeSeconds;
                NetInterClipReloadTimer.Value = InterClipReloadTimeSeconds;
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