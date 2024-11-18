using System;
using UnityEngine;

namespace TankGuns
{
    public abstract class BaseTankGun : MonoBehaviour
    {
        [field: SerializeField] public GameObject ProjectilePrefab { get; set; }

        [field: SerializeField] public Transform ShellSpawnPoint { get; set; }
        
        [field: SerializeField] public GameObject NextShellToLoadPrefab { get; set; }

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

        protected void OnReloadStart() => ReloadStartEvent?.Invoke();
        
        protected void OnReloadEnd() => ReloadEndEvent?.Invoke();
        
        protected GameObject LaunchProjectile(GameObject prefab)
        {
            var projectile = Instantiate(prefab);
            projectile.transform.position = ShellSpawnPoint.position;
            projectile.transform.rotation = ShellSpawnPoint.rotation;
            var rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = projectile.transform.forward * 20;
            return projectile;
        }
    }   
}