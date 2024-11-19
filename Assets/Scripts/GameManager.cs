using System.Collections.Generic;
using System.Linq;
using Ui;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject _deathScreen;

    private Button _retryButton;

    private GameObject _gameplayCursor;

    private IEnumerable<Tank> _tanks;
    
    private readonly Dictionary<int, Vector3> _revivePositions = new();
    
    private readonly Dictionary<int, Quaternion> _reviveRotations = new();
        
    private void Start()
    {
        _deathScreen = GameObject.Find("DeathScreen");
        _deathScreen.SetActive(false);
        _retryButton = _deathScreen.GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "RetryButton");
        _retryButton?.onClick.AddListener(OnPlayerRetry);
        _gameplayCursor = GameObject.Find("GameplayCursor");
        
        GameObject player = GameObject.Find("PlayerTank");
        player.GetComponent<Tank>().OnDeath += OnPlayerDeath;

        _tanks = FindObjectsOfType<Tank>();
        foreach (var tank in _tanks)
        {
            _revivePositions.Add(tank.GetInstanceID(), tank.transform.position);
            _reviveRotations.Add(tank.GetInstanceID(), tank.transform.rotation);
        }
    }

    private void OnPlayerDeath()
    {
        _deathScreen.SetActive(true);
        _gameplayCursor.SetActive(false);
    }

    private void OnPlayerRetry()
    {
        _deathScreen.SetActive(false);
        _gameplayCursor.SetActive(true);
        foreach (var tank in _tanks) {
            tank.Revive();
            tank.transform.position = _revivePositions[tank.GetInstanceID()];
            tank.transform.rotation = _reviveRotations[tank.GetInstanceID()];
        }
    }
}
