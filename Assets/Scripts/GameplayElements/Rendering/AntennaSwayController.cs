using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AntennaSwayController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Affects how extreme the antenna sways. Larger values mean less sway, smaller values mean more sway.")]
    public float swayWeight;

    private LineRenderer _lineRenderer;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        var offsetFromPrevious = 0f;

        // Initialize positions
        for (int idx = 0; idx < _lineRenderer.positionCount; idx++)
        {
            var curPos = _lineRenderer.GetPosition(idx);
            var anchorPos = idx > 0
               ? _lineRenderer.GetPosition(idx - 1)
               : this.transform.parent.position;
            var dependentPos = idx < _lineRenderer.positionCount - 1
               ? _lineRenderer.GetPosition(idx + 1)
               : Vector3.zero;

            anchorPos.y += offsetFromPrevious;
            _lineRenderer.SetPosition(idx, anchorPos);

            offsetFromPrevious = dependentPos.y - curPos.y;

        }
    }

    void Update()
    {
        if (!NetworkManager.Singleton.IsClient)
        {
            return;
        }

        for (int idx = 0; idx < _lineRenderer.positionCount; idx++)
        {
            var curPos = _lineRenderer.GetPosition(idx);

            var nextPos = idx > 0
                ? _lineRenderer.GetPosition(idx - 1)
                : this.transform.parent.position;

            nextPos.y = curPos.y;

            var lerpedPos = Vector3.Lerp(curPos, nextPos, swayWeight);
            _lineRenderer.SetPosition(idx, lerpedPos);
        }
    }
}
