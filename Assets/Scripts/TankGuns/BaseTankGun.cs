using System;
using UnityEngine;

namespace TankGuns
{
    public abstract class BaseTankGun : MonoBehaviour
    {
        [field: SerializeField] public GameObject ProjectilePrefab { get; set; }

        [field: SerializeField] public Transform ShellSpawnPoint { get; set; }
        
        public abstract event Action OnReloadEnd;

        protected virtual void Awake()
        {
            ShellSpawnPoint = transform.Find("ShellSpawnPoint");
        }
    
        public abstract GameObject Fire();
    
        public abstract void Reload();
    }   
}