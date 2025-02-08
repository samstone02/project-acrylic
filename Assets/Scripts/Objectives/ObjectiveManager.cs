using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [field: SerializeField] public GameObject AmmoObjectivePrefab { get; set; }
    [field: SerializeField] public GameObject LifeObjectivePrefab { get; set; }
    [field: SerializeField] public GameObject HealthObjectivePrefab { get; set; }
    [field: SerializeField] public List<GameObject> ObjectiveLocations { get; set; }
    [field: SerializeField] public float TimeBetweenObjectivesSeconds { get; set; } = 30;
    [field: SerializeField] public float ObjectiveSpawnTimeSeconds { get; set; } = 30;

    public bool ObjectiveComplete { get; private set; } = false;

    private float _objectivesTimer;

    void Start()
    {
        StartRandomObjective();
    }

    void Update()
    {
        if (ObjectiveComplete)
        {
            _objectivesTimer = TimeBetweenObjectivesSeconds;
        }
        else if (_objectivesTimer > 0)
        {
            _objectivesTimer -= Time.deltaTime;
        }
        else
        {
            StartRandomObjective();
        }
    }

    private void StartRandomObjective()
    {
        _objectivesTimer = TimeBetweenObjectivesSeconds;

        var location = GetRandomLocation();
        var objective = GetRandomObjectiveType();

        StartObjective(location, objective);
    }
    private Vector3 GetRandomLocation()
    {
        int idx = UnityEngine.Random.Range(0, ObjectiveLocations.Count);
        return ObjectiveLocations[idx].transform.position;
    }

    private ObjectiveType GetRandomObjectiveType()
    {
        int idx = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ObjectiveType)).Length);
        return (ObjectiveType)idx;
    }

    private void StartObjective(Vector3 location, ObjectiveType objectiveType)
    {
        switch (objectiveType)
        {
            case ObjectiveType.Ammo:
                StartAmmoObjective(location);
                break;
            case ObjectiveType.Life:
                StartAmmoObjective(location);
                break;
        }
    }

    private void StartAmmoObjective(Vector3 location)
    {
        var go = Instantiate(AmmoObjectivePrefab);
        var objective = go.GetComponent<AmmoObjective>();
        objective.Timer = ObjectiveSpawnTimeSeconds;
    }
}
