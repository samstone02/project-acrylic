using UnityEngine;

namespace Projectiles
{
    public class StandardShell : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }
    }
}