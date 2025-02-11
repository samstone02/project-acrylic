using System;
using System.Diagnostics;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Gameplay
{
    public class GamePlayerStopwatch : MonoBehaviour
    {
        private TextMeshProUGUI _stopwatchText;
        
        private Stopwatch _stopwatch;
        
        protected void Start()
        {
            _stopwatch = new Stopwatch();
            _stopwatchText = GetComponentInChildren<TextMeshProUGUI>();
                
            GameObject player = GameObject.Find("PlayerTank");
            var playerTank = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Tank>();
            playerTank.DeathClientEvent += () => _stopwatch.Stop();
            
            var retryButton = FindObjectsOfType<Button>(true)
                .First(b => b.name == "RetryButton")
                .GetComponent<Button>();
            retryButton.onClick.AddListener(() => _stopwatch.Restart());
            
            _stopwatch.Start();
        }

        protected void Update()
        {
            var text = string.Empty;
            
            if (_stopwatch.Elapsed.Minutes > 0)
            {
                text = _stopwatch.Elapsed.Minutes + ":";
            }
            
            text += (_stopwatch.Elapsed.Minutes > 0 && _stopwatch.Elapsed.Seconds < 10 ? "0" : string.Empty) + _stopwatch.Elapsed.Seconds + ".";
            text += _stopwatch.Elapsed.Milliseconds / 100;
            
            _stopwatchText.text = text;
        }
    }
}