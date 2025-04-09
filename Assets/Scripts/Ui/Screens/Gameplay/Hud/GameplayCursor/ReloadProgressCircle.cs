using TankGuns;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Gameplay.GameplayCursor
{
    public class ReloadProgressCircle : MonoBehaviour
    {
        private Image _image;

        private AutoLoadingCannon _playerCannon;

        private void Start()
        {
            _image = GetComponent<Image>();
            Cursor.visible = false;
            var playerTank = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Tank>();
            _playerCannon = playerTank.GetComponentInChildren<AutoLoadingCannon>();
        }
        
        private void Update()
        {
            if (_playerCannon.ReloadTimer.Value >= 0)
            {
                var reloadTimeSeconds = _playerCannon.AmmoReserve > 0 ? _playerCannon.ReloadTimeSeconds : _playerCannon.FallbackReloadTimeSeconds;

                _image.fillAmount = 1 - _playerCannon.ReloadTimer.Value / reloadTimeSeconds;   
            }
            else
            {
                _image.fillAmount = 0;
            }
        }
    }
}