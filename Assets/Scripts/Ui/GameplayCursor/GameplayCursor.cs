using UnityEngine;

namespace Ui.GameplayCursor
{
    public class GameplayCursor : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            Cursor.visible = false;
        }
        
        private void Update()
        {
            _rectTransform.position = Input.mousePosition;
        }
    }
}