using UnityEngine;

public class StandardTankGun : BaseTankGun
{
    public override GameObject Shoot()
    {
        var projectile = Instantiate(ProjectilePrefab);
        projectile.transform.position = this.transform.position;
        projectile.transform.rotation = this.transform.rotation;
        var rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(1, 1, 0);
        return projectile;
    }
}