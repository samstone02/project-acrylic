using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Dummy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            Debug.Log("Hit the dummy!");
        }
    }
}
