using TankGuns;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.GameplayCursor
{
    public class ReloadProgressCircle : MonoBehaviour
    {
        private Image _image;

        private AutoloadingTankGun _playerGun;

        private void Start()
        {
            _image = GetComponent<Image>();
            Cursor.visible = false;
            var playerTank = GameObject.Find("PlayerTank").GetComponent<Tank>();
            _playerGun = playerTank.GetComponentInChildren<AutoloadingTankGun>();
        }
        
        private void Update()
        {
            if (_playerGun.ReloadTimer >= 0)
            {
                _image.fillAmount = 1 - _playerGun.ReloadTimer / _playerGun.ReloadTimeSeconds;   
            }
            else
            {
                _image.fillAmount = 0;
            }
        }
    }
}