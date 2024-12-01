using UnityEngine;

namespace Projectiles
{
    public class RicochetShell : Shell
    {
        [field: SerializeField] public bool RicochetInfinitely { get; private set; }
        
        [field: SerializeField] public int RicochetCount { get; private set; }
        
        [field: SerializeField] public int DamageRampOnRicochet { get; private set; }
        
        [field: SerializeField] public int SpeedRampOnRicochet { get; private set; }
        
        [field: SerializeField] public float RicochetDetectionDistance { get; private set; }
        
        private Rigidbody _rigidbody;

        protected void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        protected void Update()
        {
            bool reflected = ReflectIfRaycastHit();
            
            if (!RicochetInfinitely)
            {
                if (reflected)
                {
                    RicochetCount--;
                    
                    if (RicochetCount < 0)
                    {
                        Destroy(gameObject);
                    }
                }
            }
            
            if (reflected)
            {
                Damage += DamageRampOnRicochet;
                _rigidbody.velocity = _rigidbody.velocity.normalized * (_rigidbody.velocity.magnitude + SpeedRampOnRicochet);
            }
        }

        protected bool ReflectIfRaycastHit()
        {
            var ray = new Ray(transform.position, transform.forward);
            Physics.Raycast(ray, out RaycastHit hitInfo, RicochetDetectionDistance, gameObject.layer);

            if (hitInfo.collider == null)
            {
                return false;
            }

            Vector3 reflectionForward = Vector3.Reflect(transform.forward, hitInfo.normal);
            float rotationAngle = 90 - Mathf.Atan2(reflectionForward.z, reflectionForward.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, rotationAngle, 0);
            _rigidbody.velocity = _rigidbody.velocity.magnitude * reflectionForward;

            return true;
        }
    }
}