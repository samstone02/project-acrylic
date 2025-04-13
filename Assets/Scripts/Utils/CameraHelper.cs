using UnityEngine;
using UnityEngine.InputSystem;

public static class CameraHelper
{
    public static RaycastHit RaycastFromMouse(LayerMask mask)
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        Physics.Raycast(mousePosWorld, Camera.main.transform.forward, out RaycastHit hit, 1000, mask);
        return hit;
    }
}