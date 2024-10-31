using TMPro;
using UnityEngine;

public class PlayerHealthUi : MonoBehaviour
{
    private Tank _playerTank;
    
    private TextMeshProUGUI _healthText;
    
    private void Start()
    {
        _playerTank = GameObject.Find("Player Tank").GetComponent<Tank>();
        _healthText = GetComponentInChildren<TextMeshProUGUI>();
        
        _playerTank.OnReceiveDamage += UpdateHealthIndicator;

        _healthText.text = _playerTank.hitPoints.ToString();
    }

    private void UpdateHealthIndicator()
    {
        int currentDisplayHealth = int.Parse(_healthText.text) - 1;
        currentDisplayHealth = Mathf.Clamp(currentDisplayHealth, 0, _playerTank.hitPoints);
        _healthText.text = currentDisplayHealth.ToString();
    }
}
