using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(Camera))]
public class SpectatorCameraController : MonoBehaviour
{
    public float movementSpeed;

    [field: SerializeField] public InputActionReference Move { get; private set; }

    private Camera _camera;

    private void OnEnable()
    {
        Move.action.Enable();
    }

    private void OnDisable()
    {
        Move.action.Disable();
    }

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        EndSpectating();
        this.transform.parent = null;
    }

    private void Update()
    {
        var move = Move.action.ReadValue<Vector2>();

        this.transform.position += movementSpeed * Time.deltaTime * new Vector3(move.x, 0, move.y);
    }

    public void BeginSpectating()
    {
        _camera.enabled = true;
    }

    public void EndSpectating()
    {
        _camera.enabled = false;
    }
}