﻿using UnityEngine;
using UnityEngine.Serialization;

public class StandardTankGun : BaseTankGun
{
    [SerializeField] public int magazineCapacity;

    [SerializeField] public float reloadTimeSeconds;

    private int _shellsInMagazine;
    
    private bool _isReloading;
    
    private float _reloadTimer;

    protected override void Awake()
    {
        base.Awake();
        
        _shellsInMagazine = magazineCapacity;
    }

    private void Update()
    {
        if (_isReloading)
        {
            if (_reloadTimer > 0)
            {
                _reloadTimer -= Time.deltaTime;   
            }
            else
            {
                _shellsInMagazine = magazineCapacity;
                _isReloading = false;
            }
        }
    }
    
    public override GameObject Fire()
    {
        if (_isReloading || _shellsInMagazine <= 0)
        {
            return null;
        }

        GameObject projectile = LaunchProjectile();
        _shellsInMagazine--;
        
        ReloadIfMagazineEmpty();
        
        return projectile;
    }

    public override void Reload()
    {
        if (_isReloading || _shellsInMagazine == magazineCapacity)
        {
            return;
        }
        
        _isReloading = true;
        _reloadTimer = reloadTimeSeconds;
    }

    private GameObject LaunchProjectile()
    {
        var projectile = Instantiate(ProjectilePrefab);
        projectile.transform.position = ShellSpawnPoint.position;
        projectile.transform.rotation = ShellSpawnPoint.rotation;
        var rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = projectile.transform.forward * 20;
        return projectile;
    }

    private void ReloadIfMagazineEmpty()
    {
        if (_shellsInMagazine == 0)
        {
            Reload();
        }
    }
}