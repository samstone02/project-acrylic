using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaceLandMineAbility : BaseTankAbility
{
    [field: SerializeField] public NetworkObject LandMinePrefab { get; private set; }

    public override void OnTrigger(Tank tank)
    {
        var playerAimMask = LayerMask.GetMask("Player Aim");

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        Physics.Raycast(mousePosWorld, Camera.main.transform.forward, out RaycastHit hit, 100, playerAimMask);

        NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(LandMinePrefab, position: hit.point);
    }
}