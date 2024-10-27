using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerTankAgent : BaseTankAgent
{
    [SerializeField] public InputActionReference fireInput;
    
    [SerializeField] public InputActionReference reloadInput;

    [SerializeField] public InputActionReference leftTrackRollInput;

    [SerializeField] public InputActionReference rightTrackRollInput;
    
    [SerializeField]
    [Tooltip("In degrees per second.")]
    public float turretRotationSpeed = 120f;
    
    [SerializeField]
    [Tooltip("How close is close enough.")]
    public float turretMinDiffAngle = 1f;
    
    private Camera _mainCamera;

    private MeshCollider _ground;

    private void OnEnable()
    {
        fireInput.action.Enable();
        reloadInput.action.Enable();
        leftTrackRollInput.action.Enable();
        rightTrackRollInput.action.Enable();
    }

    private void OnDisable()
    {
        fireInput.action.Disable();
        reloadInput.action.Disable();
        leftTrackRollInput.action.Disable();
        rightTrackRollInput.action.Disable();
    }
    
    protected new void Start()
    {
        base.Start();

        _mainCamera = Camera.main;
    }

    public override bool GetDecisionFire()
    {
        return fireInput.action.triggered;
    }
    
    public override bool GetDecisionReload()
    {
        return reloadInput.action.triggered;
    }

    public override float GetDecisionRotateTurret()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mousePosWorld = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        Physics.Raycast(mousePosWorld, _mainCamera.transform.forward, out RaycastHit hit, 100);
        
        Vector3 targetDirection = hit.point - Turret.transform.position;
        targetDirection.Normalize();
        Vector3 turretDirection = Turret.transform.forward;
        turretDirection.Normalize();
        
        Vector3 turretTargetCross = Vector3.Cross(turretDirection, targetDirection);
        
        if (Vector3.Angle(targetDirection, turretDirection) < turretMinDiffAngle)
        {
            return 0.0f;
        }
        
        int direction = turretTargetCross.y > 0 ? 1 : -1;
        Turret.transform.Rotate(Vector3.up, direction * turretRotationSpeed * Time.deltaTime);

        return 1.0f;
    }

    public override (float, float) GetDecisionRollTracks()
    {
        float left = leftTrackRollInput.action.ReadValue<float>();
        float right = rightTrackRollInput.action.ReadValue<float>();
        return (left, right);
    }
}
