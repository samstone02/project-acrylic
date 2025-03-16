using TMPro;
using UnityEngine;

public class PlayerLivesUi : MonoBehaviour
{
    private TextMeshProUGUI _livesText;
    private Tank _player;

    void Start()
    {
        _livesText = GetComponent<TextMeshProUGUI>();
        _player = NetcodeHelper.GetLocalClientTankOrNull();

        _player.DeathClientEvent += HandleDeathClientEvent;
        _player.AddLivesClientEvent += HandleAddLivesClientEvent;

        _livesText.text = _player.Lives.ToString();
    }

    private void OnDestroy()
    {
        _player.DeathClientEvent -= HandleDeathClientEvent;
        _player.AddLivesClientEvent -= HandleAddLivesClientEvent;
    }

    private void HandleDeathClientEvent() => _livesText.text = _player.Lives.ToString();

    private void HandleAddLivesClientEvent() => _livesText.text = _player.Lives.ToString();
}
