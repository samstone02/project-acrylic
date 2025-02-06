using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Ui.Gameplay
{
    public class PlayerHealthUi : MonoBehaviour
    {
        private Tank _playerTank;
    
        private TextMeshProUGUI _healthText;
    
        private void Start()
        {
            _playerTank = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Tank>();
            _healthText = GetComponentInChildren<TextMeshProUGUI>();
        
            _playerTank.DamagedEvent += HandlePlayerTankDamage;
            _playerTank.RevivalEvent += PlayerRevive;

            _healthText.text = _playerTank.HealthCapacity.ToString();
        }

        private void HandlePlayerTankDamage(int damage)
        {
            int currentDisplayHealth = int.Parse(_healthText.text) - damage;
            currentDisplayHealth = Mathf.Clamp(currentDisplayHealth, 0, _playerTank.HealthCapacity);
            _healthText.text = currentDisplayHealth.ToString();
        }

        private void PlayerRevive()
        {
            _healthText.text = _playerTank.HealthCapacity.ToString();
        }
    }
   
}