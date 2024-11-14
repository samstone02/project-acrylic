using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class DebugManager : MonoBehaviour
{
    [field: SerializeField] public bool ShouldRenderNavPaths { get; set; }

    private LineRenderer _lineRenderer;

    private GameObject _player;
    
    private List<GameObject> _tankObjects;
    
    protected void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _player = GameObject.Find("PlayerTank");
        _tankObjects = FindObjectsOfType<Tank>().Select(t => t.gameObject).ToList();
    }
    
    protected void Update()
    {
        if (ShouldRenderNavPaths)
        {
            RenderNavPaths();
        }
    }

    private void RenderNavPaths()
    {
        foreach (var go in _tankObjects)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(go.transform.position, _player.transform.position, NavMesh.AllAreas, path);
            
            if (_lineRenderer is { enabled:true } && path.corners.Length > 1)
            {
                _lineRenderer.positionCount = path.corners.Length;
                _lineRenderer.SetPositions(path.corners);
            }
        }
    }
}
