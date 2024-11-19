using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [field: SerializeField] public GameObject EnemyPrefab { get; set; }

    [field: SerializeField] public float RespawnDelaySeconds { get; set; } = 5f;

    public GameObject Instance { get; private set; }

    protected void Awake()
    {
        SpawnTank();
        Instance.GetComponent<Tank>().OnDeath += OnTankDeath;
    }
    
    private void OnTankDeath() 
    {
        Invoke(nameof(SpawnTank), RespawnDelaySeconds);
    }

    private void SpawnTank()
    {
        Instance ??= Instantiate(EnemyPrefab);
        Instance.GetComponent<Tank>().Revive();
        Instance.transform.position = transform.position;
        Instance.transform.rotation = transform.rotation;
    }
}