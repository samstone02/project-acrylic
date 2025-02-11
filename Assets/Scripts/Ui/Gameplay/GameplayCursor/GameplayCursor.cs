using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Gameplay.GameplayCursor
{
    public class GameplayCursor : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            Cursor.visible = false;

            var gameplayUi = GetComponentInParent<GameplayUiManager>();
            Button retryButton = gameplayUi.GetComponentsInChildren<Button>(true).First(b => b.name == "RetryButton");
            retryButton.onClick.AddListener(() => Cursor.visible = false);
            
            var playerTank = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Tank>();
            playerTank.DeathClientEvent += () => Cursor.visible = true;
        }
        
        private void Update()
        {
            _rectTransform.position = Input.mousePosition;
        }
    }
}