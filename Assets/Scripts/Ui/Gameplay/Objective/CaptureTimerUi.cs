using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaptureTimerUi : MonoBehaviour
{
    [SerializeField] private Color BlueProgressCircleColor;
    [SerializeField] private Color OrangeProgressCircleColor;

    private TMP_Text _countdownText;
    private Image _progressCircle;

    private ObjectiveManager _objectiveManager;
    private Objective _selectedObjective;

    private void Start()
    {
        _countdownText = GetComponentInChildren<TMP_Text>();
        _progressCircle = GetComponentInChildren<Image>();

        _objectiveManager = FindObjectsByType<ObjectiveManager>(FindObjectsSortMode.None).First();
        _objectiveManager.ObjectiveSelectedClientEvent += HandleObjectiveSelected;

        _progressCircle.fillAmount = 0;
    }

    private void Update()
    {
        if (_selectedObjective == null)
        {
            _countdownText.text = "";
            _progressCircle.fillAmount = 0;
        }
        else if (_selectedObjective.PrepTimer <= 0)
        {
            if (_selectedObjective.ControllingTeam == Team.Blue)
            {
                _progressCircle.fillClockwise = false;
                _progressCircle.color = BlueProgressCircleColor;
            }
            else if (_selectedObjective.ControllingTeam == Team.Orange)
            {
                _progressCircle.fillClockwise = true;
                _progressCircle.color = OrangeProgressCircleColor;
            }

            _countdownText.text = _selectedObjective.CaptureTimer.ToString("0.0");
            _progressCircle.fillAmount = (_selectedObjective.CaptureTimeSeconds - _selectedObjective.CaptureTimer) / _selectedObjective.CaptureTimeSeconds;
        }
    }

    private void HandleObjectiveSelected(ulong selectedObjectiveNetworkObjectId)
    {
        _selectedObjective = FindObjectsByType<Objective>(FindObjectsSortMode.None)
            .First(o => o.NetworkObject.NetworkObjectId == selectedObjectiveNetworkObjectId);
        _selectedObjective.ObjectiveCapturedClientEvent += HandleObjectiveCaptured;
    }

    private void HandleObjectiveCaptured()
    {
        _selectedObjective = null;
    }
}
