using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectiveManager : NetworkBehaviour
{
    [field: SerializeField] public List<NetworkObject> ObjectivePrefabs { get; private set; } = new List<NetworkObject>();
    [field: SerializeField] public float TimeBetweenObjectivesSeconds { get; private set; } = 30;
    [field: SerializeField] public List<Vector3> ObjectiveLocations { get; private set; } = new List<Vector3>();

    private float _objectivesTimer;

    private BaseObjective _currentObjective;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartRandomObjective();
        }
    }

    void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (_currentObjective == null && _objectivesTimer > 0)
        {
            _objectivesTimer -= Time.deltaTime;
            _objectivesTimer = Mathf.Clamp(_objectivesTimer, 0, TimeBetweenObjectivesSeconds); 
        }

        if (_objectivesTimer <= 0)
        {
            StartRandomObjective();
        }
    }

    private void StartRandomObjective()
    {
        _objectivesTimer = TimeBetweenObjectivesSeconds;

        var location = GetRandomLocation();
        var objectivePrefab = GetRandomObjectiveType();

        StartObjective(location, objectivePrefab);
    }

    private Vector3 GetRandomLocation()
    {
        int idx = UnityEngine.Random.Range(0, ObjectiveLocations.Count);
        return ObjectiveLocations[idx];
    }

    private NetworkObject GetRandomObjectiveType()
    {
        int idx = UnityEngine.Random.Range(0, ObjectivePrefabs.Count);
        return ObjectivePrefabs[idx];
    }

    private void StartObjective(Vector3 location, NetworkObject objectivePrefab)
    {
        NetworkLog.LogInfoServer($"Started a new objective: {objectivePrefab.name}.");
        var go = NetworkManager.SpawnManager.InstantiateAndSpawn(objectivePrefab, position: location);
        _currentObjective = go.GetComponent<BaseObjective>();
        _currentObjective.ObjectiveCapturedEvent += (_) => {
            _objectivesTimer = TimeBetweenObjectivesSeconds;
            _currentObjective = null;
        };
    }
}
