using System.Collections;
using System.Collections.Generic;
using TankGuns;
using TMPro;
using UnityEngine;

namespace Ui
{
    public class PlayerAmmoUi : MonoBehaviour
    {
        private TextMeshProUGUI _ammoText;

        private AutoloadingTankGun _autoloadingTankGun;
    
        private void Start()
        {
            _ammoText = GetComponentInChildren<TextMeshProUGUI>();
            _autoloadingTankGun = GameObject.Find("PlayerTank").GetComponentInChildren<AutoloadingTankGun>();

            
            _ammoText.text = _autoloadingTankGun.MagazineCapacity.ToString();
            
            _autoloadingTankGun.FireEvent += OnPlayerFireEvent;
            _autoloadingTankGun.ReloadStartEvent += OnPlayerReloadStartEvent;
            _autoloadingTankGun.ReloadEndEvent += OnPlayerReloadEndEvent;
        }

        private void OnPlayerFireEvent()
        {
            int ammoCount = int.Parse(_ammoText.text) - 1;
            _ammoText.text = Mathf.Clamp(ammoCount, 0, _autoloadingTankGun.MagazineCapacity).ToString();
        }

        private void OnPlayerReloadStartEvent()
        {
            _ammoText.text = "...";
        }

        private void OnPlayerReloadEndEvent()
        {
            _ammoText.text = _autoloadingTankGun.MagazineCapacity.ToString();
        }
    }
}