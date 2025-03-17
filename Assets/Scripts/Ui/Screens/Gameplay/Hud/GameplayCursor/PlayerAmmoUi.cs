using TankGuns;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Ui.Gameplay
{
    public class PlayerAmmoUi : MonoBehaviour
    {
        private TextMeshProUGUI _ammoText;

        private BaseCannon _cannon;

        private void Start()
        {
            var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
            var tank = localPlayer.GetComponent<Tank>();

            _cannon = localPlayer.GetComponentInChildren<BaseCannon>();
            _ammoText = GetComponent<TextMeshProUGUI>();

            _ammoText.text = _cannon.AmmoReserve.ToString();

            _cannon.FireClientEvent.AddListener(OnPlayerFireEvent);
            _cannon.ReloadStartEvent.AddListener(OnPlayerReloadStartEvent);
            _cannon.ReloadEndEvent.AddListener(OnPlayerReloadEndEvent);
            _cannon.AmmoRefillClientEvent.AddListener(OnPlayerRefillAmmo);
        }

        private void OnPlayerFireEvent()
        {
            _ammoText.text = _cannon.AmmoReserve.ToString();
        }

        private void OnPlayerReloadStartEvent()
        {
            _ammoText.text = _cannon.AmmoReserve.ToString();
        }

        private void OnPlayerReloadEndEvent()
        {
            _ammoText.text = _cannon.AmmoReserve.ToString();
        }

        private void OnPlayerRefillAmmo()
        {
            _ammoText.text = _cannon.AmmoReserve.ToString();
        }
    }
}