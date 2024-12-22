using System;
using Projectiles;
using UnityEngine;

namespace TankGuns
{
    public abstract class BaseCannon : MonoBehaviour
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
    
        public abstract GameObject Fire();
    
        public abstract void Reload();
        
        protected void OnFire() => FireEvent?.Invoke();

        protected void InvokeReloadStart() => ReloadStartEvent?.Invoke();
        
        protected void InvokeReloadEnd() => ReloadEndEvent?.Invoke();
        
        protected GameObject LaunchProjectile(GameObject prefab)
        {
            var projectile = Instantiate(prefab);
            projectile.transform.position = ShellSpawnPoint.position;
            projectile.transform.rotation = ShellSpawnPoint.rotation;
            var rb = projectile.GetComponent<Rigidbody>();
            var shell = projectile.GetComponent<Shell>().Speed;
            rb.linearVelocity = projectile.transform.forward * shell;
            return projectile;
        }
    }   
}