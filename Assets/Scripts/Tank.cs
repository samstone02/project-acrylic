using System;
using Projectiles;
using UnityEngine;
using TankAgents;
using TankGuns;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class Tank : NetworkBehaviour
{
    [field: SerializeField] public int HitPointCapacity { get; set; } = 3;
    
    [field: SerializeField]
    [Tooltip("In degrees per second.")]
    public float TurretRotationSpeed { get; set; } = 120f;
    
    [field: SerializeField] public float TreadTorque { get; set; } = 10f;
    
    private Transform LeftTrackRollPosition { get; set; }
    
    private Transform RightTrackRollPosition { get; set; }
    
    public event Action<int> OnReceiveDamage;
    
    public event Action OnDeath;
    
    public event Action OnRevival;
    
    private Rigidbody _rigidbody;

    private BaseTankAgent _agent;

    private BaseCannon _cannon;
    
    private GameObject _turret;
    
    private int _currentHitPoints;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _agent = GetComponent<BaseTankAgent>();
        _cannon = GetComponentInChildren<BaseCannon>();
        _turret = transform.Find("Turret").gameObject;
        
        _currentHitPoints = HitPointCapacity;
        
        LeftTrackRollPosition = transform.Find("LeftTrackRollPosition");
        RightTrackRollPosition = transform.Find("RightTrackRollPosition");
    }

    [Rpc.SendTo(Server)]
    private void ServerAwake()
    {
        
    }
    
    private void Update()
    {
        if (_agent is null || _currentHitPoints <= 0)
        {
            return;
        }
        
        bool fireDecision = _agent.GetDecisionFire();
        bool reloadDecision = _agent.GetDecisionReload();
        
        if (fireDecision)
        {
            GameObject projectile = _cannon.Fire();
        }
        if (reloadDecision)
        {
            _cannon.Reload();
        }
        
        float rotationDirection = _agent.GetDecisionRotateTurret();
        _turret.transform.Rotate(Vector3.up, rotationDirection * TurretRotationSpeed * Time.deltaTime);  
        
        (float left, float right) = _agent.GetDecisionRollTracks();
        
        Move(left, right);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            var shell = collision.gameObject.GetComponent<Shell>();
            TakeDamage(shell.Damage);
        }
    }

    private void Move(float left, float right)
    {
        _rigidbody.AddForceAtPosition(left * TreadTorque * Time.deltaTime * transform.forward, LeftTrackRollPosition.position);
        _rigidbody.AddForceAtPosition(right * TreadTorque * Time.deltaTime * transform.forward, RightTrackRollPosition.position);
    }

    public void TakeDamage(int damage)
    {
        if (_currentHitPoints == 0)
        {
            return;
        }
        
        _currentHitPoints -= damage;
        OnReceiveDamage?.Invoke(damage); 
        
        if (_currentHitPoints == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
    

    public void Revive()
    {
        OnRevival?.Invoke();
        
        _currentHitPoints = HitPointCapacity;
    }
}
