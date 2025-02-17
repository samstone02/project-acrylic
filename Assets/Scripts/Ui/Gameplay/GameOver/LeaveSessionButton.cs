using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Gameplay.GameOver
{
    public class LeaveSessionButton : MonoBehaviour
    {
        public Action OnLeaveSession;

        private void Start()
        {
            transform.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            OnLeaveSession?.Invoke();
        }
    }
}