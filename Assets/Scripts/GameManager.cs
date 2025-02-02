using System.Collections.Generic;
using System.Linq;
using Ui;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [field: SerializeField] Transform PlayerSpawnPoint { get; set; }
    
    private GameObject _deathScreen;

    private Button _retryButton;

    private GameObject _gameplayCursor;
    
    private GameObject _player;
        
    private void Start()
    {
        _deathScreen = GameObject.Find("DeathScreen");
        _deathScreen.SetActive(false);
        _retryButton = _deathScreen.GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "RetryButton");
        _retryButton?.onClick.AddListener(OnPlayerRetry);
        _gameplayCursor = GameObject.Find("GameplayCursor");
        
        _player = GameObject.Find("PlayerTank");
        _player.GetComponent<Tank>().OnDeath += OnPlayerDeath;
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
        _player.GetComponent<Tank>().ReviveRpc();
        _player.transform.SetPositionAndRotation(PlayerSpawnPoint.position, PlayerSpawnPoint.rotation);
    }
}
