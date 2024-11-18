using TankGuns;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.GameplayCursor
{
    public class ReloadProgressCircle : MonoBehaviour
    {
        private Image _image;

        private AutoLoadingCannon _playerCannon;

        private void Start()
        {
            _image = GetComponent<Image>();
            Cursor.visible = false;
            var playerTank = GameObject.Find("PlayerTank").GetComponent<Tank>();
            _playerCannon = playerTank.GetComponentInChildren<AutoLoadingCannon>();
        }
        
        private void Update()
        {
            if (_playerCannon.ReloadTimer >= 0)
            {
                _image.fillAmount = 1 - _playerCannon.ReloadTimer / _playerCannon.ReloadTimeSeconds;   
            }
            else
            {
                _image.fillAmount = 0;
            }
        }
    }
}