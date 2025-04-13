using UnityEngine;

// TODO: make this client only

[RequireComponent(typeof(LineRenderer))]
public class AimLaserController : MonoBehaviour
{
    public float MaxRaycastDistance = 500f;

    private LineRenderer _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        var ray = new Ray(transform.position, transform.forward);
        var didHit = Physics.Raycast(ray, out var hit, MaxRaycastDistance);

        _lineRenderer.SetPosition(0, transform.position);

        if (didHit)
        {
            _lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            _lineRenderer.SetPosition(0, transform.forward * MaxRaycastDistance);
        }
    }
}