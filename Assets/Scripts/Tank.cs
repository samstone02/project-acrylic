using System;
using Projectiles;
using TankAgents;
using TankGuns;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Tank : NetworkBehaviour
{
    [field: SerializeField] public int HealthCapacity { get; set; } = 3;
    
    [field: SerializeField]
    [Tooltip("In degrees per second.")]
    public float TurretRotationSpeed { get; set; } = 120f;
    
    [field: SerializeField] public float TreadTorque { get; set; } = 10f;

    [field: SerializeField] public GameObject AgentPrefab { get; set; }

    [field: SerializeField] public GameObject GameplayUi { get; set; }
    
    
    public event Action<int> DamagedEvent;

    public event Action<int> HealedEvent;
    
    public event Action DeathEvent;
    
    public event Action RevivalEvent;

    private Transform LeftTrackRollPosition { get; set; }

    private Transform RightTrackRollPosition { get; set; }

    private Rigidbody _rigidbody { get; set; }

    private BaseTankAgent _agent { get; set; }

    private BaseCannon _cannon { get; set; }

    private GameObject _turret { get; set; }

    private NetworkVariable<int> _healthNetVar = new NetworkVariable<int>();
    
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
        if (IsServer)
        {
            _healthNetVar.Value = HealthCapacity;
        }

        if (IsOwner)
        {
            var agent = Instantiate(AgentPrefab, transform);
            _agent = agent.GetComponent<BaseTankAgent>();
            _rigidbody.isKinematic = false;
            Instantiate(GameplayUi);
            _healthNetVar.OnValueChanged += OnHealthNetVarChanged;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            var shell = collision.gameObject.GetComponent<Shell>();
            TakeDamage(shell.Damage);
        }
    }

    protected void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (_agent is null || _healthNetVar.Value <= 0)
        {
            return;
        }

        bool fireDecision = _agent.GetDecisionFire();
        bool reloadDecision = _agent.GetDecisionReload();

        if (fireDecision)
        {
             _cannon.Fire();
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

    public void Revive()
    {
        RevivalEvent?.Invoke();
        ReviveRpc();
    }

    public void TakeDamage(int damage)
    {
        if (IsServer)
        {
            if (_healthNetVar.Value == 0)
            {
                return;
            }

            _healthNetVar.Value = Mathf.Clamp(_healthNetVar.Value - damage, 0, int.MaxValue);

            if (_healthNetVar.Value == 0)
            {
                Die();
            }
        }
        else
        {
            Debug.LogWarning("Only the server should call this method.");
        }
    }

    private void Move(float left, float right, float deltaTime)
    {
        _rigidbody.AddForceAtPosition(left * TreadTorque * deltaTime * transform.forward, LeftTrackRollPosition.position);
        _rigidbody.AddForceAtPosition(right * TreadTorque * deltaTime * transform.forward, RightTrackRollPosition.position);
    }

    private void OnHealthNetVarChanged(int previous, int next)
    {
        if (previous > next)
        {
            DamagedEvent?.Invoke(previous - next);

            if (next <= 0)
            {
                DeathEvent?.Invoke();
            }
        }
        else if (previous < next)
        {
            HealedEvent?.Invoke(next - previous);
        }
    }

    private void Die()
    {
        DeathEvent?.Invoke();
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

    [Rpc(SendTo.Server)]
    private void ReviveRpc()
    {
        _healthNetVar.Value = HealthCapacity;
    }

    [Rpc(SendTo.Server)]
    private void MoveRpc(float left, float right, float deltaTime)
    {
        Move(left, right, deltaTime);
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

}
