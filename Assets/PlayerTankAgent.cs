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
        // Theory:
        // 1. [X] Get the Turret's forward vector
        // 2. [ ] Get the vector pointing to the mouse
        //      [ ] Get the mouse position in world space
        // 3. [ ] Get the cross product
        // 4. [ ] Rotate clockwise or counterclockwise based on the cross product
        
        Vector2 mp1 = Mouse.current.position.ReadValue();
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(new Vector3(mp1.x, 0, mp1.y));
        
        var contact = _ground.Raycast(_mainCamera.ScreenPointToRay(new Vector3(mp1.x, 0, mp1.y)), out RaycastHit hit, 100);
        
        Debug.Log(contact);
        //
        // Debug.Log(mousePos);
        
        Vector3 mouseDirection = mousePos - transform.position;
        Vector3 turretDirection = _turret.transform.forward.normalized;
        
        Vector3 axis = Vector3.Cross(turretDirection, mouseDirection);

        // if (axis.y > 0)
        // {
        //     Debug.Log("Y > 0");
        //     _turret.transform.Rotate(Vector3.up, 90 * Time.deltaTime);
        // }
        // else if (axis.y < 0)
        // {
        //     Debug.Log("Y > 0");
        //     _turret.transform.Rotate(Vector3.up, -90 * Time.deltaTime);
        // }
        
        return 1.0f;
    }

    public override (float, float) GetDecisionMoveTreads()
    {
        float left =  leftTreadRoll.action.ReadValue<float>();
        float right =  rightTreadRoll.action.ReadValue<float>();
        return (left, right);
    }
}
