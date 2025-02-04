using System;
using Projectiles;
using Unity.Netcode;
using UnityEngine;

namespace TankGuns
{
    public abstract class BaseCannon : NetworkBehaviour
    {
        [field: SerializeField] public GameObject ProjectilePrefab { get; set; }

        [field: SerializeField] public Transform ShellSpawnPoint { get; set; }

        public event Action FireClientEvent;

        public event Action ReloadStartEvent;

        public event Action ReloadEndEvent;

        protected virtual void Awake()
        {
            ShellSpawnPoint = transform.Find("ShellSpawnPoint");
        }

        public abstract void Fire();

        public abstract void Reload();

        protected void OnFire()
        {
            if (IsClient)
            {
                FireClientEvent?.Invoke();
            }
        }

        protected void OnReloadStart() => ReloadStartEvent?.Invoke();

        protected void OnReloadEnd() => ReloadEndEvent?.Invoke();
        
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