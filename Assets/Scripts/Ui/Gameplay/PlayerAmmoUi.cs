using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TankGuns;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Ui.Gameplay
{
    public class PlayerAmmoUi : MonoBehaviour
    {
        private TextMeshProUGUI _ammoText;

        private TextMeshProUGUI _loadedAmmoText;

        private BaseCannon _cannon;

        private void Start()
        {
            var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
            var tank = localPlayer.GetComponent<Tank>();

            _cannon = localPlayer.GetComponentInChildren<BaseCannon>();
            _ammoText = GetComponentsInChildren<TextMeshProUGUI>().First(c => c.name == "Ammo Text");
            _loadedAmmoText = GetComponentsInChildren<TextMeshProUGUI>().First(c => c.name == "Loaded Ammo Text");

            _ammoText.text = _cannon.CurrentAmmo.ToString();

            if (_cannon is not AutoLoadingCannon)
            {
                _loadedAmmoText.text = "...";
            }
            else
            {
                _loadedAmmoText.text = "N/A";
            }

            _cannon.FireClientEvent += OnPlayerFireEvent;
            _cannon.ReloadStartEvent += OnPlayerReloadStartEvent;
            _cannon.ReloadEndEvent += OnPlayerReloadEndEvent;
            _cannon.AmmoRefillClientEvent += OnPlayerRefillAmmo;
        }

        private void OnPlayerFireEvent()
        {
            if (_cannon is AutoLoadingCannon autoLoader)
            {
                // subtract 1 because this is a client event
                // the current magazine count won't be updated yet
                _loadedAmmoText.text = (autoLoader.MagazineCount - 1).ToString();
            }
            else
            {
                // subtract 1 because this is a client event
                // the current ammo count won't be updated yet
                _ammoText.text = (_cannon.CurrentAmmo - 1).ToString();
            }
        }

        private void OnPlayerReloadStartEvent()
        {
            if (_cannon is AutoLoadingCannon autoLoader)
            {
                _ammoText.text = _cannon.CurrentAmmo.ToString();

                _loadedAmmoText.text = "...";
            }
        }

        private void OnPlayerReloadEndEvent()
        {
            if (_cannon is AutoLoadingCannon autoLoader)
            {
                _ammoText.text = (_cannon.CurrentAmmo - autoLoader.MagazineCapacity).ToString();
                _loadedAmmoText.text = autoLoader.MagazineCapacity.ToString();
            }
        }

        private void OnPlayerRefillAmmo()
        {
            if (_cannon is AutoLoadingCannon autoLoader)
            {
                _ammoText.text = (_cannon.CurrentAmmo - autoLoader.MagazineCapacity).ToString();
            }
        }
    }
}