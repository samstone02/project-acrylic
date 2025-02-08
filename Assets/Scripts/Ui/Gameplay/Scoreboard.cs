using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Gameplay
{
    public class Scoreboard : MonoBehaviour
    {
        public Action OnRetry;

        private void Start()
        {
            transform.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            OnRetry?.Invoke();
        }
    }
}