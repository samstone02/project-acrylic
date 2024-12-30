using System;
using Projectiles;
using TankAgents;
using TankGuns;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.GridBrushBase;

[RequireComponent(typeof(Rigidbody))]
public class Tank : NetworkBehaviour
{
    [field: SerializeField] public int HitPointCapacity { get; set; } = 3;
    
    [field: SerializeField]
    [Tooltip("In degrees per second.")]
    public float TurretRotationSpeed { get; set; } = 120f;
    
    [field: SerializeField] public float TreadTorque { get; set; } = 10f;

    [field: SerializeField] public GameObject AgentPrefab { get; set; }
    
    private Transform LeftTrackRollPosition { get; set; }
    
    private Transform RightTrackRollPosition { get; set; }
    
    public event Action<int> OnReceiveDamage;
    
    public event Action OnDeath;
    
    public event Action OnRevival;
    
    private Rigidbody _rigidbody;

    private BaseTankAgent _agent;

    private BaseCannon _cannon;
    
    private GameObject _turret;
    
    //private int _currentHitPoints;

    private NetworkVariable<int> _currentHitpoints = new NetworkVariable<int>();
    
    protected void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _cannon = GetComponentInChildren<BaseCannon>();
        _turret = transform.Find("Turret").gameObject;
                
        LeftTrackRollPosition = transform.Find("LeftTrackRollPosition");
        RightTrackRollPosition = transform.Find("RightTrackRollPosition");
    }

    public override void OnNetworkSpawn()
    {
        _currentHitpoints.Value = HitPointCapacity;

        if (IsOwner)
        {
            var agent = Instantiate(AgentPrefab, transform);
            _agent = agent.GetComponent<BaseTankAgent>();
        }
    }
    
    protected void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (_agent is null || _currentHitpoints.Value <= 0)
        {
            return;
        }

        bool fireDecision = _agent.GetDecisionFire();
        bool reloadDecision = _agent.GetDecisionReload();

        if (fireDecision)
        {
             _cannon.FireRpc();
        }
        if (reloadDecision)
        {
            _cannon.Reload();
        }

        Vector3 targetDirection = _agent.GetDecisionRotateTurret();
        RotateTurretRpc(targetDirection, Time.deltaTime);

        (float left, float right) = _agent.GetDecisionRollTracks();

        MoveRpc(left, right, Time.deltaTime);
    }

    [Rpc(SendTo.Server)]
    private void RotateTurretRpc(Vector3 targetDirection, float deltaTime)
    {
        Vector3 currentDirection = _turret.transform.forward;
        currentDirection.y = 0;
        currentDirection.Normalize();

        float rotationDirection = CalculateTurretRotationDirection(targetDirection, currentDirection, TurretRotationSpeed);

        _turret.transform.Rotate(Vector3.up, rotationDirection * TurretRotationSpeed * deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            var shell = collision.gameObject.GetComponent<Shell>();
            TakeDamageRpc(shell.Damage);
        }
    }

    [Rpc(SendTo.Server)]
    private void MoveRpc(float left, float right, float deltaTime)
    {
        _rigidbody.AddForceAtPosition(left * TreadTorque * deltaTime * transform.forward, LeftTrackRollPosition.position);
        _rigidbody.AddForceAtPosition(right * TreadTorque * deltaTime * transform.forward, RightTrackRollPosition.position);
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageRpc(int damage)
    {
        if (_currentHitpoints.Value == 0)
        {
            return;
        }
        
        _currentHitpoints.Value = Mathf.Clamp(_currentHitpoints.Value - damage, 0, int.MaxValue);
        OnReceiveDamage?.Invoke(damage); 
        
        if (_currentHitpoints.Value == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }

    [Rpc(SendTo.Server)]
    public void ReviveRpc()
    {
        OnRevival?.Invoke();
        
        _currentHitpoints.Value = HitPointCapacity;
    }

    /// <summary>
    /// Returns the rotation direction as a percentage of the rotation speed.
    /// Positive values are clockwise, negative values are counterclockwise.
    /// </summary>
    private static float CalculateTurretRotationDirection(
        Vector3 targetDirection,
        Vector3 currentDirection,
        float rotationSpeed)
    {
        Vector3 turretTargetCross = Vector3.Cross(currentDirection, targetDirection);

        int direction = turretTargetCross.y > 0 ? 1 : -1;
        float angleDifference = Vector3.Angle(currentDirection, targetDirection);
        float angleRotation = rotationSpeed * Time.deltaTime;

        if (angleDifference < angleRotation)
        {
            return angleDifference / angleRotation * direction;
        }

        return direction;
    }
}
