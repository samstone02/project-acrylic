using System;
using UnityEngine;

namespace TankGuns
{
    public abstract class BaseTankGun : MonoBehaviour
    {
        [field: SerializeField] public GameObject ProjectilePrefab { get; set; }

        [field: SerializeField] public Transform ShellSpawnPoint { get; set; }
        
        public GameObject NextShellToLoadPrefab { get; set; }
        
        public abstract event Action OnReloadEnd;

        protected virtual void Awake()
        {
            ShellSpawnPoint = transform.Find("ShellSpawnPoint");
        }
    
        public abstract GameObject Fire();
    
        public abstract void StartReload();
        
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