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

            _player = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
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

            if (PlayerSpawnPoint != null)
            {
                _player.transform.SetPositionAndRotation(PlayerSpawnPoint.position, PlayerSpawnPoint.rotation);

            }
        }
    }

}
