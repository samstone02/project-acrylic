using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ObjectiveManager : NetworkBehaviour
{
    [field: SerializeField] public float TimeBetweenObjectivesSeconds { get; private set; } = 30;

    [field: SerializeField] private List<Objective> Objectives = new List<Objective>();

    public event Action<ulong> ObjectiveSelectedClientEvent;

    private float _objectivesTimer;

    private Objective _currentObjective;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Objectives = FindObjectsByType<Objective>(FindObjectsSortMode.None).ToList();
            _objectivesTimer = TimeBetweenObjectivesSeconds;
        }
    }

    void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (_currentObjective == null)
        {
            if (_objectivesTimer > 0)
            {
                _objectivesTimer -= Time.deltaTime;
                _objectivesTimer = Mathf.Clamp(_objectivesTimer, 0, TimeBetweenObjectivesSeconds);
            }

            if (_objectivesTimer <= 0)
            {
                StartRandomObjective();
            }
        }
    }

    private void StartRandomObjective()
    {
        var objective = GetRandomObjective();
        var objectiveType = GetRandomObjectiveType(objective);

        StartObjective(objective, objectiveType);
    }

    private Objective GetRandomObjective()
    {
        int idx = UnityEngine.Random.Range(0, Objectives.Count);
        return Objectives[idx];
    }

    private BaseObjectiveType GetRandomObjectiveType(Objective objective)
    {
        var objectiveTypes = objective.GetComponentsInChildren<BaseObjectiveType>();
        int idx = UnityEngine.Random.Range(0, objectiveTypes.Length);
        return objectiveTypes[idx];
    }

    private void StartObjective(Objective selectedObjective, BaseObjectiveType selectedObjetiveType)
    {
        NetworkLog.LogInfoServer($"Started a new objective: {selectedObjective.name}.");

        _currentObjective = selectedObjective;
        _currentObjective.ObjectiveCapturedServerEvent += (team) => {
            _objectivesTimer = TimeBetweenObjectivesSeconds;
            _currentObjective = null;
        };
        _currentObjective.PrepObjective(selectedObjetiveType);

        ObjectiveSelectedClientRpc(_currentObjective.NetworkObjectId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ObjectiveSelectedClientRpc(ulong selectedObjectiveNetworkObjectId)
    {
        ObjectiveSelectedClientEvent?.Invoke(selectedObjectiveNetworkObjectId);
    }
}
