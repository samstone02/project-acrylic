using System.Collections;
using System.Collections.Generic;
using TankGuns;
using TMPro;
using UnityEngine;

namespace Ui.Gameplay
{
    public class PlayerAmmoUi : MonoBehaviour
    {
        private TextMeshProUGUI _ammoText;

        private AutoLoadingCannon _autoLoadingCannon;
    
        private void Start()
        {
            _ammoText = GetComponentInChildren<TextMeshProUGUI>();
            _autoLoadingCannon = GameObject.Find("PlayerTank").GetComponentInChildren<AutoLoadingCannon>();
            
            _ammoText.text = _autoLoadingCannon.MagazineCapacity.ToString();
            
            _autoLoadingCannon.FireEvent += OnPlayerFireEvent;
            _autoLoadingCannon.ReloadStartEvent += OnPlayerReloadStartEvent;
            _autoLoadingCannon.ReloadEndEvent += OnPlayerReloadEndEvent;
        }

        private void OnPlayerFireEvent()
        {
            int ammoCount = int.Parse(_ammoText.text) - 1;
            _ammoText.text = Mathf.Clamp(ammoCount, 0, _autoLoadingCannon.MagazineCapacity).ToString();
        }

        private void OnPlayerReloadStartEvent()
        {
            _ammoText.text = "...";
        }

        private void OnPlayerReloadEndEvent()
        {
            _ammoText.text = _autoLoadingCannon.MagazineCapacity.ToString();
        }
    }
}