using System.Collections.Generic;
using System.Linq;
using Ui;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Canvas _deathScreen;

    private Button _retryButton;

    private List<Tank> _tanks;
        
    private void Start()
    {
        _deathScreen = GameObject.Find("DeathScreen").GetComponent<Canvas>();
        _deathScreen.enabled = false;
        _retryButton = _deathScreen.GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "RetryButton");
        _retryButton?.onClick.AddListener(OnPlayerRetry);
        
        GameObject.Find("PlayerTank").GetComponent<Tank>().OnDeath += OnPlayerDeath;
        _tanks = FindObjectsOfType<Tank>().ToList();
    }

    private void OnPlayerDeath()
    {
        _deathScreen.enabled = true;
    }

    private void OnPlayerRetry()
    {
        _deathScreen.enabled = false;
        _tanks.ForEach(x => x.Respawn());
    }
}
