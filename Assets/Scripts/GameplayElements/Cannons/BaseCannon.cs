﻿using System;
using Projectiles;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace TankGuns
{
    public abstract class BaseCannon : NetworkBehaviour
    {
        [field: SerializeField] public int AmmoCapacity { get; set; }

        [field: SerializeField] public int StartingAmmo { get; set; }

        [field: SerializeField] public GameObject ProjectilePrefab { get; set; }

        [field: SerializeField] public GameObject FallbackProjectilePrefab { get; set; }

        [field: SerializeField] public Transform ShellSpawnPoint { get; set; }

        [field: SerializeField] public UnityEvent FireClientEvent { get; private set; }

        [field: SerializeField] public UnityEvent ReloadStartEvent { get; private set; }

        [field: SerializeField] public UnityEvent ReloadEndEvent { get; private set; }

        [field: SerializeField] public UnityEvent AmmoRefillClientEvent { get; private set; }

        /// <summary>
        /// The amount of total ammo left for the cannon to fire.
        /// </summary>
        public int AmmoReserve { get => _ammoReserveNetVar.Value; }

        private readonly NetworkVariable<int> _ammoReserveNetVar = new NetworkVariable<int>();

        protected virtual void Awake()
        {
            ShellSpawnPoint = transform.Find("ShellSpawnPoint");
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _ammoReserveNetVar.Value = Math.Min(StartingAmmo, AmmoCapacity);
            }
            if (IsClient)
            {
                _ammoReserveNetVar.OnValueChanged += OnAmmoReserveNetVarChanged;
            }
        }

        public virtual void Fire()
        {
            FireServerRpc();
            FireClientEvent?.Invoke();
        }

        public abstract void Reload();

        protected void OnReloadStart() => ReloadStartEvent?.Invoke();

        protected void OnReloadEnd() => ReloadEndEvent?.Invoke();

        public void FillAmmo(int count)
        {
            // TODO: The player can have greater than the ammo capacity if they have shells loaded into their autoloader.
            // Is this something that needs to be fixed?
            if (IsServer)
            {
                _ammoReserveNetVar.Value += count;
                _ammoReserveNetVar.Value = Mathf.Clamp(_ammoReserveNetVar.Value, 0, AmmoCapacity);

                if (_ammoReserveNetVar.Value - count == 0)
                {
                    Reload();
                }
            }
        }

        [Rpc(SendTo.Server)]
        private void FireServerRpc()
        {
            _ammoReserveNetVar.Value = Mathf.Clamp(--_ammoReserveNetVar.Value, 0, int.MaxValue);
        }

        private void OnAmmoReserveNetVarChanged(int previous, int current)
        {
            if (previous < current)
            {
                AmmoRefillClientEvent?.Invoke();
            }
        }
        
        protected GameObject LaunchProjectile(GameObject prefab)
        {
            var networkObject = prefab.GetComponent<NetworkObject>();
            var projectile = NetworkManager.SpawnManager.InstantiateAndSpawn(
                networkObject,
                position: ShellSpawnPoint.position,
                rotation: ShellSpawnPoint.rotation).gameObject;
            var rb = projectile.GetComponent<Rigidbody>();
            var shell = projectile.GetComponent<Shell>().Speed;
            rb.linearVelocity = projectile.transform.forward * shell;
            return projectile;
        }
    }   
}