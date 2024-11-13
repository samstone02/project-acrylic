using System;
using System.Threading.Tasks;
using UnityEngine;
using TankAgents;
using TankGuns;
using UnityEditor;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class Tank : MonoBehaviour
{
    [field: SerializeField] public int HitPointCapacity { get; set; } = 3;
    
    [field: SerializeField]
    [Tooltip("In degrees per second.")]
    public float TurretRotationSpeed { get; set; } = 120f;
    
    [field: SerializeField] public float TreadTorque { get; set; } = 10f;
    
    private Transform LeftTrackRollPosition { get; set; }
    
    private Transform RightTrackRollPosition { get; set; }
    
    public event Action OnReceiveDamage;
    
    public event Action OnFire;
    
    public event Action OnReloadStart;
    
    public event Action OnReloadEnd;
    
    public event Action OnDeath;
    
    private Rigidbody _rigidbody;

    private BaseTankAgent _agent;

    private BaseTankGun _tankGun;
    
    private GameObject _turret;
    
    private int _currentHitPoints;

    private Vector3 _startPosition;
    
    private Quaternion _startRotation;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _agent = GetComponent<BaseTankAgent>();
        _tankGun = GetComponentInChildren<BaseTankGun>();
        _turret = transform.Find("Turret").gameObject;
        
        _currentHitPoints = HitPointCapacity;
        _tankGun.OnReloadEnd += GunOnReloadEnd;
        
        LeftTrackRollPosition = transform.Find("LeftTrackRollPosition");
        RightTrackRollPosition = transform.Find("RightTrackRollPosition");
        
        _startPosition = transform.position;
        _startRotation = transform.rotation;
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
            GameObject projectile = _tankGun.Fire();
            OnFire?.Invoke();
        }
        if (reloadDecision)
        {
            _tankGun.Reload();
            OnReloadStart?.Invoke();
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
            _currentHitPoints--;
            OnReceiveDamage?.Invoke(); 
        }

        if (_currentHitPoints == 0)
        {
            Die();
        }
    }

    private void Move(float left, float right)
    {
        _rigidbody.AddForceAtPosition(left * TreadTorque * Time.deltaTime * transform.forward, LeftTrackRollPosition.position);
        _rigidbody.AddForceAtPosition(right * TreadTorque * Time.deltaTime * transform.forward, RightTrackRollPosition.position);
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }

    public void Respawn()
    {
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        _currentHitPoints = HitPointCapacity;
    }

    private void GunOnReloadEnd()
    {
        OnReloadEnd?.Invoke();
    }
}
