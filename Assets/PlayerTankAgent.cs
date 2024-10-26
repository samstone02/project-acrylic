using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerTankAgent : BaseTankAgent
{
    [SerializeField] public InputActionReference shoot;

    [SerializeField] public InputActionReference leftTreadRoll;

    [SerializeField] public InputActionReference rightTreadRoll;

    private Camera _mainCamera;

    private MeshCollider _ground;

    [SerializeField]
    [Tooltip("In degrees per second.")]
    public float turretRotationSpeed = 120f;
    
    [SerializeField]
    [Tooltip("How close is close enough.")]
    public float turretMinDiffAngle = 1f;

    private void OnEnable()
    {
        shoot.action.Enable();
        leftTreadRoll.action.Enable();
        rightTreadRoll.action.Enable();
    }

    private void OnDisable()
    {
        shoot.action.Disable();
        leftTreadRoll.action.Disable();
        rightTreadRoll.action.Disable();
    }
    
    protected new void Start()
    {
        base.Start();
        
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    public override bool GetDecisionShoot()
    {
        return shoot.action.triggered;
    }

    public override float GetDecisionRotateTurret()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mousePosWorld = _mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        Physics.Raycast(mousePosWorld, Camera.main.transform.forward, out RaycastHit hit, 100);
        
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

    public override (float, float) GetDecisionMoveTreads()
    {
        float left = leftTreadRoll.action.ReadValue<float>();
        float right = rightTreadRoll.action.ReadValue<float>();
        return (left, right);
    }
}
