using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [field: SerializeField] public GameObject EnemyPrefab { get; set; }

    [field: SerializeField] public float RespawnDelaySeconds { get; set; } = 5f;

    [field: SerializeField] public List<Vector3> SpawnPoints { get; set; }

    private float _timer;

    protected void Start()
    {
        var playerTank = GameObject.Find("PlayerTank").GetComponent<Tank>();
        playerTank.RevivalEvent += () => FindObjectsByType<Tank>(FindObjectsSortMode.None)
            .Where(t => t != playerTank)
            .ToList()
            .ForEach(tank => Destroy(tank.gameObject));
    }

    protected void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            SpawnTank();
            _timer = RespawnDelaySeconds;
        }
    }

    private void SpawnTank()
    {
        GameObject instance = Instantiate(EnemyPrefab);
        Vector3 spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
        instance.transform.position = spawnPoint;
    }
}