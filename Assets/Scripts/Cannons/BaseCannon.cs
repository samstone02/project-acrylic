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

        public event Action FireEvent;
        
        public event Action ReloadStartEvent;
        
        public event Action ReloadEndEvent;
        
        protected virtual void Awake()
        {
            ShellSpawnPoint = transform.Find("ShellSpawnPoint");
        }
    
        public abstract void FireRpc();
    
        public abstract void Reload();
        
        protected void OnFire() => FireEvent?.Invoke();

        protected void InvokeReloadStart() => ReloadStartEvent?.Invoke();
        
        protected void InvokeReloadEnd() => ReloadEndEvent?.Invoke();
        
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