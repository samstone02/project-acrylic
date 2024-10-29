using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BaseTankAgent))]
public class Tank : MonoBehaviour
{
    [SerializeField] public int hitPoints = 3;
    
    [SerializeField] public float treadTorque = 10f;
    
    private Rigidbody _rigidbody;

    private BaseTankAgent _agent;

    private BaseTankGun _tankGun;
    
    private GameObject _turret;
    
    private int _currentHitPoints;
    
    private void Awake()
    {
        Debug.Log("Hello, Tanks!");
        
        _rigidbody = GetComponent<Rigidbody>();
        _agent = GetComponent<BaseTankAgent>();
        _tankGun = GetComponentInChildren<BaseTankGun>();
        _turret = transform.Find("Turret").gameObject;
        
        _currentHitPoints = hitPoints;
    }
    
    private void Update()
    {
        bool fireDecision = _agent.GetDecisionFire();
        bool reloadDecision = _agent.GetDecisionReload();
        
        if (fireDecision)
        {
            var projectile = _tankGun.Fire();
        }
        if (reloadDecision)
        {
            Debug.Log("Player decided to reload");
            _tankGun.Reload();
        }
        
        var x = _agent.GetDecisionRotateTurret();
        
        (float left, float right) = _agent.GetDecisionRollTracks();
        
        Move(left, right);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            _currentHitPoints--;
        }

        if (_currentHitPoints == 0)
        {
            Debug.Log("You died!");
        }
    }

    private void Move(float left, float right)
    {
        // Currently a physics based movement system.
        // I'm not sure if I want to keep this because it's hard to get it feeling right.
        // I'll leave it for now.
        // It feels heavy and slow. Even though it's a tank, I don't want this to be the case.
        // There's basically no acceleration. It's just top speed right away. And the top speed is low.
        // I'll have to play with it more.
        // I think what I'm missing is friction?
        
        if (left > 0 && right > 0)
        {
            _rigidbody.AddRelativeForce(2 * treadTorque * Time.deltaTime * Vector3.forward, ForceMode.Force);
        }
        else if (left < 0 && right < 0)
        {
            _rigidbody.AddRelativeForce(2 * treadTorque * Time.deltaTime * Vector3.back, ForceMode.Force);
        }
        else if (left > 0 && right < 0)
        {
            _rigidbody.AddRelativeTorque(2 * treadTorque * Time.deltaTime * Vector3.up, ForceMode.Force);
        }
        else if (left < 0 && right > 0)
        {
            _rigidbody.AddRelativeTorque(2 * treadTorque * Time.deltaTime * Vector3.down, ForceMode.Force);
        }
        else if (left == 0 && right > 0)
        {
            _rigidbody.AddRelativeForce(1.25f * treadTorque * Time.deltaTime * Vector3.forward, ForceMode.Force);
            _rigidbody.AddRelativeTorque(0.75f * treadTorque * Time.deltaTime * Vector3.down, ForceMode.Force);
        }
        else if (left > 0 && right == 0)
        {
            _rigidbody.AddRelativeForce(1.25f * treadTorque * Time.deltaTime * Vector3.forward, ForceMode.Force);
            _rigidbody.AddRelativeTorque(0.75f * treadTorque * Time.deltaTime * Vector3.up, ForceMode.Force);
        }
        else if (left < 0 && right == 0)
        {
            _rigidbody.AddRelativeForce(1.25f * treadTorque * Time.deltaTime * Vector3.back, ForceMode.Force);
            _rigidbody.AddRelativeTorque(0.75f * treadTorque * Time.deltaTime * Vector3.down, ForceMode.Force);
        }
        else if (left == 0 && right < 0)
        {
            _rigidbody.AddRelativeForce(1.25f * treadTorque * Time.deltaTime * Vector3.back, ForceMode.Force);
            _rigidbody.AddRelativeTorque(0.75f * treadTorque * Time.deltaTime * Vector3.up, ForceMode.Force);
        }
    }
}
