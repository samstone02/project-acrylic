using System.Linq;
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
            
            var ui = GameObject.Find("GameplayUi");
            Button retryButton = ui.GetComponentsInChildren<Button>(true).First(b => b.name == "RetryButton");
            retryButton.onClick.AddListener(() => Cursor.visible = false);
            
            var playerTank = GameObject.Find("PlayerTank").GetComponent<Tank>();
            playerTank.OnDeath += () => Cursor.visible = true;
        }
        
        private void Update()
        {
            _rectTransform.position = Input.mousePosition;
        }
    }
}