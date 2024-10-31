using UnityEngine;

namespace TankGuns
{
    public abstract class BaseTankGun : MonoBehaviour
    {
        [field: SerializeField]
        public GameObject ProjectilePrefab { get; set; }

        protected Transform ShellSpawnPoint;

        protected virtual void Awake()
        {
            ShellSpawnPoint = transform.Find("ShellSpawnPoint");
        }
    
        public abstract GameObject Fire();
    
        public abstract void Reload();
    }   
}