using UnityEngine;

public class StandardTankGun : BaseTankGun
{
    public override GameObject Shoot()
    {
        var projectile = Instantiate(ProjectilePrefab);
        projectile.transform.position = BulletSpawnPoint.position;
        projectile.transform.rotation = BulletSpawnPoint.rotation;
        var rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 5;
        return projectile;
    }
}