using Unity.Netcode;
using UnityEngine;

namespace Projectiles
{
    public class RicochetShell : Shell
    {
        [field: SerializeField] public bool RicochetInfinitely { get; private set; }

        [field: SerializeField] public int RicochetCount { get; private set; }

        [field: SerializeField] public int DamageRampOnRicochet { get; private set; }

        [field: SerializeField] public int SpeedRampOnRicochet { get; private set; }

        private Rigidbody _rigidbody;

        protected void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            if (!NetworkManager.IsServer)
            {
                return;
            }

            if (collision.gameObject.GetComponent<Tank>() != null)
            {
                var tank = collision.gameObject.GetComponent<Tank>();
                tank.TakeDamage(Damage);
                Destroy(gameObject);
                return;
            }

            if (!RicochetInfinitely)
            {
                RicochetCount--;

                if (RicochetCount < 0)
                {
                    Destroy(gameObject);
                }
            }

            var firstContact = GetFirstContact(collision);
            ReflectSelf(firstContact);

            // Damage and speed ramp
            Damage += DamageRampOnRicochet;
            _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * (_rigidbody.linearVelocity.magnitude + SpeedRampOnRicochet);
        }

        private ContactPoint GetFirstContact(Collision collision)
        {
            var contactPoints = new ContactPoint[collision.contactCount];
            collision.GetContacts(contactPoints);

            Vector3 trailPoint = transform.position + (transform.forward * -10);
            ContactPoint firstContact = default;
            float minDistance = float.MaxValue;

            Debug.DrawLine(transform.position, trailPoint, Color.red, 5f);

            // Get the "first contact point"
            foreach (var contactPoint in contactPoints)
            {
                Debug.DrawLine(transform.position, contactPoint.point, Color.red, 5f);

                float distanceFromTrailPoint = (trailPoint - contactPoint.point).magnitude;

                if (distanceFromTrailPoint < minDistance)
                {
                    firstContact = contactPoint;
                    minDistance = distanceFromTrailPoint;
                }
            }

            return firstContact;
        }

        private void ReflectSelf(ContactPoint firstContact)
        {
            Vector3 reflectionForward = Vector3.Reflect(transform.forward, firstContact.normal);
            float rotationAngle = 90 - Mathf.Atan2(reflectionForward.z, reflectionForward.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, rotationAngle, 0);
            _rigidbody.linearVelocity = Speed * reflectionForward;
            _rigidbody.angularVelocity = Vector3.zero; // override any rotation
        }
    }
}