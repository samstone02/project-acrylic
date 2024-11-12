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
        // Currently a physics based movement system.
        // I'm not sure if I want to keep this because it's hard to get it feeling right.
        // I'll leave it for now.
        // It feels heavy and slow. Even though it's a tank, I don't want this to be the case.
        // There's basically no acceleration. It's just top speed right away. And the top speed is low.
        // I'll have to play with it more.
        // I think what I'm missing is friction?
        
        if (left > 0 && right > 0)
        {
            _rigidbody.AddRelativeForce(2 * TreadTorque * Time.deltaTime * Vector3.forward, ForceMode.Force);
        }
        else if (left < 0 && right < 0)
        {
            _rigidbody.AddRelativeForce(2 * TreadTorque * Time.deltaTime * Vector3.back, ForceMode.Force);
        }
        else if (left > 0 && right < 0)
        {
            _rigidbody.AddRelativeTorque(2 * TreadTorque * Time.deltaTime * Vector3.up, ForceMode.Force);
        }
        else if (left < 0 && right > 0)
        {
            _rigidbody.AddRelativeTorque(2 * TreadTorque * Time.deltaTime * Vector3.down, ForceMode.Force);
        }
        else if (left == 0 && right > 0)
        {
            _rigidbody.AddRelativeForce(1.25f * TreadTorque * Time.deltaTime * Vector3.forward, ForceMode.Force);
            _rigidbody.AddRelativeTorque(0.75f * TreadTorque * Time.deltaTime * Vector3.down, ForceMode.Force);
        }
        else if (left > 0 && right == 0)
        {
            _rigidbody.AddRelativeForce(1.25f * TreadTorque * Time.deltaTime * Vector3.forward, ForceMode.Force);
            _rigidbody.AddRelativeTorque(0.75f * TreadTorque * Time.deltaTime * Vector3.up, ForceMode.Force);
        }
        else if (left < 0 && right == 0)
        {
            _rigidbody.AddRelativeForce(1.25f * TreadTorque * Time.deltaTime * Vector3.back, ForceMode.Force);
            _rigidbody.AddRelativeTorque(0.75f * TreadTorque * Time.deltaTime * Vector3.down, ForceMode.Force);
        }
        else if (left == 0 && right < 0)
        {
            _rigidbody.AddRelativeForce(1.25f * TreadTorque * Time.deltaTime * Vector3.back, ForceMode.Force);
            _rigidbody.AddRelativeTorque(0.75f * TreadTorque * Time.deltaTime * Vector3.up, ForceMode.Force);
        }
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
