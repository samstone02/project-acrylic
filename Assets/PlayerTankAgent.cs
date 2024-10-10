using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
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
    
    protected new void Start()
    {
        base.Start();
        
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _ground = GameObject.Find("Ground").GetComponent<MeshCollider>();
    }

    public override bool GetDecisionShoot()
    {
        return shoot.action.triggered;
    }

    public override float GetDecisionRotateTurret()
    {
        Vector2 mp1 = Mouse.current.position.ReadValue();
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(new Vector3(mp1.x, mp1.y, 25));
        
        Vector3 mouseDirection = mousePos - transform.position;
        mouseDirection.y = 0;
        mouseDirection.Normalize();
        Vector3 turretDirection = _turret.transform.forward;
        turretDirection.y = 0;
        turretDirection.Normalize();
        
        Vector3 axis = Vector3.Cross(turretDirection, mouseDirection);
        
        if (Vector3.Angle(mouseDirection, turretDirection) < turretMinDiffAngle)
        {
            return 0.0f;
        }
        
        int direction = axis.y > 0 ? 1 : -1;
        _turret.transform.Rotate(Vector3.up, direction * turretRotationSpeed * Time.deltaTime);

        return 1.0f;
    }

    public override (float, float) GetDecisionMoveTreads()
    {
        float left =  leftTreadRoll.action.ReadValue<float>();
        float right =  rightTreadRoll.action.ReadValue<float>();
        return (left, right);
    }
}
