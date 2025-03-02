using System;
using Projectiles;
using TankAgents;
using TankGuns;
using Unity.Collections;
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

    [field: SerializeField] public int AmmoCapacity { get; set; } = 30;

    [field: SerializeField] public int StartingLives { get; set; } = 3;

    [field: SerializeField] public GameObject AgentPrefab { get; set; }

    [field: SerializeField] public GameObject MainCamera { get; set; }

    public float Health { get => _healthNetVar.Value; }
    public int Lives { get => _numLivesNetVar.Value; }
    
    public event Action<float> DamagedEvent;

    public event Action<float> HealedEvent;
    
    public event Action DeathClientEvent;

    public event Action DeathServerEvent;
    
    public event Action RevivalClientEvent;

    public event Action AddLivesClientEvent;

    private Transform LeftTrackRollPosition { get; set; }

    private Transform RightTrackRollPosition { get; set; }

    private Rigidbody _rigidbody { get; set; }

    private BaseTankAgent _agent { get; set; }

    private BaseCannon _cannon { get; set; }

    private GameObject _turret { get; set; }

    private readonly NetworkVariable<float> _healthNetVar = new NetworkVariable<float>();

    private readonly NetworkVariable<int> _numLivesNetVar = new NetworkVariable<int>();
    
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
            _numLivesNetVar.Value = StartingLives;
        }

        if (IsOwner)
        {
            var agent = Instantiate(AgentPrefab, transform);
            _agent = agent.GetComponent<BaseTankAgent>();
            _rigidbody.isKinematic = false;
            _healthNetVar.OnValueChanged += OnHealthNetVarChanged;
            _numLivesNetVar.OnValueChanged += OnNumLivesNetVarChanged;

            var mainCameraPos = transform.Find("MainCameraPosition").transform;
            Instantiate(MainCamera, mainCameraPos);
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
        RevivalClientEvent?.Invoke();
        ReviveRpc();
    }

    public void TakeDamage(float damage)
    {
        if (IsServer)
        {
            if (_healthNetVar.Value == 0)
            {
                return;
            }

            // OnHealthNetVarChanged is called immediately on assignment to _healthNetVar.Value on the server/host.
            // In order to prevent the `DeathClientEvent` from occuring when the tank has > 0 health the new value is stored in a temp var.
            // If the next health is zero, we fire the first decrement the lives THEN set the health to zero.
            var nextHealth = Mathf.Clamp(_healthNetVar.Value - damage, 0, int.MaxValue);

            if (nextHealth == 0)
            {
                _numLivesNetVar.Value--;
                DeathServerEvent?.Invoke();
            }

            _healthNetVar.Value = nextHealth;
        }
        else
        {
            Debug.LogWarning("Only the server should call this method.");
        }
    }

    public void FillAmmo(int count)
    {
        _cannon.FillAmmo(count);
    }

    public void AddLives(int count)
    {
        _numLivesNetVar.Value += count;
    }

    public void DieImmediate()
    {
        DieImmediateServerRpc();
    }

    [Rpc(SendTo.Server)]
    public void DieImmediateServerRpc()
    {
        _numLivesNetVar.Value = 0;
        _healthNetVar.Value = 0;
        DeathServerEvent?.Invoke();
    }

    private void Move(float left, float right, float deltaTime)
    {
        _rigidbody.AddForceAtPosition(left * TreadTorque * deltaTime * transform.forward, LeftTrackRollPosition.position);
        _rigidbody.AddForceAtPosition(right * TreadTorque * deltaTime * transform.forward, RightTrackRollPosition.position);
    }

    private void OnHealthNetVarChanged(float previous, float next)
    {
        if (previous > next)
        {
            DamagedEvent?.Invoke(previous - next);

            if (next <= 0)
            {
                DeathClientEvent?.Invoke();
            }
        }
        else if (previous < next)
        {
            HealedEvent?.Invoke(next - previous);
        }
    }

    private void OnNumLivesNetVarChanged(int previous, int next)
    {
        if (previous < next)
        {
            AddLivesClientEvent?.Invoke();
        }
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
        if (_numLivesNetVar.Value == 0)
        {
            NetworkLog.LogInfoServer($"{NetworkManager.LocalClientId} cannot revive because they have no lives left.");
            return;
        }
        _healthNetVar.Value = HealthCapacity;
        ReviveClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ReviveClientRpc()
    {
        RevivalClientEvent?.Invoke();
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
