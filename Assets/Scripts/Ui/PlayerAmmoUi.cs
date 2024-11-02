using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Ui
{
    public class PlayerAmmoUi : MonoBehaviour
    {
        private TextMeshProUGUI _ammoText;
        
        private Tank _playerTank;
    
        private void Start()
        {
            _ammoText = GetComponentInChildren<TextMeshProUGUI>();
            _playerTank = GameObject.Find("Player Tank").GetComponent<Tank>();
            
            _ammoText.text = _playerTank.MagazineCapacity.ToString();
            
            _playerTank.OnFire += OnPlayerFire;
            _playerTank.OnReloadStart += OnPlayerReloadStart;
            _playerTank.OnReloadEnd += OnPlayerReloadEnd;
        }

        private void OnPlayerFire()
        {
            int ammoCount = int.Parse(_ammoText.text) - 1;
            _ammoText.text = Mathf.Clamp(ammoCount, 0, _playerTank.MagazineCapacity).ToString();
        }

        private void OnPlayerReloadStart()
        {
            _ammoText.text = "...";
        }

        private void OnPlayerReloadEnd()
        {
            _ammoText.text = _playerTank.MagazineCapacity.ToString();
        }
    }
}