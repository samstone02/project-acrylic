using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [field: SerializeField] public int RespawnTimeSeconds {  get; private set; }

    private List<Transform> _blueSpawnPoints;
    private List<Transform> _orangeSpawnPoints;
    private TeamManager _teamManager;

    void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        _teamManager = FindObjectsByType<TeamManager>(FindObjectsSortMode.None).First();
    }

    public void FindSpawnPoints()
    {
        var spawnPoints = GameObject.Find("SpawnPoints").transform;
        _blueSpawnPoints = spawnPoints.Find("BlueSpawnPoints").GetComponentsInChildren<Transform>().ToList();
        _orangeSpawnPoints = spawnPoints.Find("OrangeSpawnPoints").GetComponentsInChildren<Transform>().ToList();
    }

    public void Respawn(Tank tank)
    {
        NetworkLog.LogInfoServer("Respawn!");

        var teamSpawnPoints = _teamManager.GetTeam(tank.OwnerClientId) == Team.Blue
            ? _blueSpawnPoints
            : _orangeSpawnPoints;
        var spawnPoint = teamSpawnPoints[new System.Random().Next(teamSpawnPoints.Count)];
       if (tank.Health <= 0)
        {
            tank.Revive();
        }
        tank.GetComponent<Rigidbody>().position = spawnPoint.position;
    }

    public void RespawnAfterDelay(Tank tank)
    {
        IEnumerator RespawnCoroutine()
        {
            yield return new WaitForSeconds(RespawnTimeSeconds);
            Respawn(tank);
        }

        StartCoroutine(RespawnCoroutine());
    }
}
