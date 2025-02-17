using System.Linq;
using TMPro;
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

        private GameObject _gameOverScreen;

        private GameObject _hud;

        private Button _retryButton;

        private GameObject _gameplayCursor;

        private TextMeshProUGUI _winLossText;

        private Tank _playerTank;

        private GameManager gameManager;

        private TeamManager teamManager;

        private void Start()
        {
            var overlay = GameObject.Find("GameplayOverlayCanvas");

            _deathScreen = overlay.transform.Find("DeathScreen").gameObject;
            _deathScreen.SetActive(false);

            _retryButton = _deathScreen.GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "RetryButton");
            _retryButton?.onClick.AddListener(OnPlayerRetry);

            _gameplayCursor = overlay.transform.Find("GameplayCursor").gameObject;

            _outOfLivesScreen = overlay.transform.Find("OutOfLivesScreen").gameObject;
            _outOfLivesScreen.SetActive(false);

            _hud = overlay.transform.Find("Hud").gameObject;
            _hud.SetActive(true);

            _gameOverScreen = overlay.transform.Find("GameOverScreen").gameObject;
            _gameOverScreen.SetActive(false);

            _winLossText = _gameOverScreen.transform.Find("WinLossText").GetComponent<TextMeshProUGUI>();

            var leaveSessionButton = _gameOverScreen.transform.Find("LeaveSessionButton").GetComponent<Button>();
            leaveSessionButton.onClick.AddListener(OnPlayerLeaveSession); 

            _playerTank = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.GetComponent<Tank>();
            _playerTank.GetComponent<Tank>().DeathClientEvent += OnPlayerDeath;

            gameManager = FindFirstObjectByType<GameManager>();
            gameManager.DeclareWinnerClientEvent += OnWinnerDeclared;

            teamManager = FindFirstObjectByType<TeamManager>();
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

        private void OnWinnerDeclared(Team winningTeam)
        {
            _deathScreen.SetActive(false);
            _hud.SetActive(false);
            _outOfLivesScreen.SetActive(false);
            _gameOverScreen.SetActive(true);

            var thisTeam = teamManager.GetTeam(NetworkManager.Singleton.LocalClientId);
            if (thisTeam == winningTeam)
            {
                _winLossText.text = "Victory!";
            }
            else
            {
                _winLossText.text = "Defeat...";
            }
        }

        private void OnPlayerLeaveSession()
        {
            gameManager.LeaveSession();
        }
    }
}
