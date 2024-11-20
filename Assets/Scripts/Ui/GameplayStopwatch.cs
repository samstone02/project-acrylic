using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ui
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
            Tank playerTank = player.GetComponent<Tank>();
            playerTank.OnDeath += () => _stopwatch.Stop();
            
            var retryButton = GameObject.Find("DeathScreen").GetComponentsInChildren<Button>().FirstOrDefault(x => x.name == "RetryButton");
            retryButton?.onClick.AddListener(() => _stopwatch.Restart());
            
            _stopwatch.Start();
        }

        protected void Update()
        {
            _stopwatchText.text = _stopwatch.Elapsed.TotalSeconds + " s";
        }
    }
}