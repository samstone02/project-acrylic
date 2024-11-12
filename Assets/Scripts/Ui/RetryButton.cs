using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
{
    public class RetryButton : MonoBehaviour
    {
        public Action OnRetry;
        
        private GameObject _gameManager;
        
        private void Start()
        {
            _gameManager = GameObject.Find("GameManager");
            transform.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            OnRetry?.Invoke();
        }
    }
}