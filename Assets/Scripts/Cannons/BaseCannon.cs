using System;
using Projectiles;
using Unity.Netcode;
using UnityEngine;

namespace TankGuns
{
    public abstract class BaseCannon : NetworkBehaviour
    {
        [field: SerializeField] public int AmmoCapacity { get; set; }

        [field: SerializeField] public GameObject ProjectilePrefab { get; set; }

        [field: SerializeField] public Transform ShellSpawnPoint { get; set; }

        public int CurrentAmmo { get => _ammoNetVar.Value; }

        public event Action FireClientEvent;

        public event Action ReloadStartEvent;

        public event Action ReloadEndEvent;

        public event Action AmmoRefillClientEvent;

        private readonly NetworkVariable<int> _ammoNetVar = new NetworkVariable<int>();

        protected virtual void Awake()
        {
            _ammoNetVar.Value = AmmoCapacity;
            ShellSpawnPoint = transform.Find("ShellSpawnPoint");

            _ammoNetVar.OnValueChanged += OnAmmoNetVarChanged;
        }

        public virtual void Fire()
        {
            ReloadServerRpc();
            FireClientEvent?.Invoke();
        }

        public abstract void Reload();

        protected void OnReloadStart() => ReloadStartEvent?.Invoke();

        protected void OnReloadEnd() => ReloadEndEvent?.Invoke();

        public void FillAmmo(int count)
        {
            if (IsServer)
            {
                _ammoNetVar.Value += count;
                _ammoNetVar.Value = Mathf.Clamp(_ammoNetVar.Value, 0, AmmoCapacity);
            }
        }

        [Rpc(SendTo.Server)]
        private void ReloadServerRpc()
        {
            _ammoNetVar.Value--;
        }

        private void OnAmmoNetVarChanged(int previous, int current)
        {
            if (previous == 0)
            {
                AmmoRefillClientEvent?.Invoke();
                Reload();
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