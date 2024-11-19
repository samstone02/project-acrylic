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