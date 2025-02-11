using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Gameplay
{
    public class GameplayUiManager : MonoBehaviour
    {
        [field: SerializeField] Transform PlayerSpawnPoint { get; set; }

        private GameObject _deathScreen;

        private GameObject _outOfLivesScreen;

        private GameObject _hud;

        private Button _retryButton;

        private GameObject _gameplayCursor;

        private Tank _playerTank;

        private void Start()
        {
            _deathScreen = GameObject.Find("DeathScreen");
            _deathScreen.SetActive(false);
            _retryButton = _deathScreen.GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "RetryButton");
            _retryButton?.onClick.AddListener(OnPlayerRetry);
            _gameplayCursor = GameObject.Find("GameplayCursor");
            _outOfLivesScreen = GameObject.Find("OutOfLivesScreen");
            _outOfLivesScreen.SetActive(false);
            _hud = GameObject.Find("Hud");
            _hud.SetActive(true);

            _playerTank = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<Tank>();
            _playerTank.GetComponent<Tank>().DeathClientEvent += OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            _gameplayCursor.SetActive(false);
            _hud.SetActive(false);

            if (_playerTank.Lives == 0)
            {
                _outOfLivesScreen.SetActive(true);
            }
            else
            {
                _deathScreen.SetActive(true);
            }
        }

        private void OnPlayerRetry()
        {
            _deathScreen.SetActive(false);
            _gameplayCursor.SetActive(true);
            _hud.SetActive(true);
            _playerTank.Revive();

            if (PlayerSpawnPoint != null)
            {
                _playerTank.transform.SetPositionAndRotation(PlayerSpawnPoint.position, PlayerSpawnPoint.rotation);
            }
        }
    }

}
