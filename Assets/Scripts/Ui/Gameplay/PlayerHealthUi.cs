using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Ui.Gameplay
{
    public class PlayerHealthUi : MonoBehaviour
    {
        private Tank _playerTank;
    
        private TextMeshProUGUI _healthText;

        private TextMeshProUGUI _livesText;
    
        private void Start()
        {
            _playerTank = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Tank>();
            _healthText = GetComponentsInChildren<TextMeshProUGUI>().First(c => c.name == "HealthText");
            _livesText = GetComponentsInChildren<TextMeshProUGUI>().First(c => c.name == "LivesText");
        
            _playerTank.DamagedEvent += HandlePlayerTakeDamage;
            _playerTank.RevivalClientEvent += PlayerRevive;
            _playerTank.DeathClientEvent += HandlePlayerDie;

            _healthText.text = _playerTank.Health.ToString();
            _livesText.text = _playerTank.Lives.ToString();
        }

        private void HandlePlayerTakeDamage(int damage)
        {
            int currentDisplayHealth = int.Parse(_healthText.text) - damage;
            currentDisplayHealth = Mathf.Clamp(currentDisplayHealth, 0, _playerTank.HealthCapacity);
            _healthText.text = currentDisplayHealth.ToString();
        }

        private void HandlePlayerDie()
        {
            _livesText.text = _playerTank.Lives.ToString();
        }

        private void PlayerRevive()
        {
            _healthText.text = _playerTank.HealthCapacity.ToString();
        }
    }
   
}