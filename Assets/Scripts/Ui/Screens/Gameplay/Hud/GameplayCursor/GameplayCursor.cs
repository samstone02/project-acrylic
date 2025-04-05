using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Gameplay.GameplayCursor
{
    public class GameplayCursor : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private void OnEnable()
        {
            Cursor.visible = false;
        }
        
        private void OnDisable()
        {
            Cursor.visible = true;
        }

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        
        private void Update()
        {
            _rectTransform.position = Input.mousePosition;
        }
    }
}