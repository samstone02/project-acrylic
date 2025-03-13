using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PrepTimerUi : MonoBehaviour
{
    private TMP_Text _countdownText;

    private ObjectiveManager _objectiveManager;
    private Objective _selectedObjective;

    void Start()
    {
        _countdownText = GetComponentInChildren<TMP_Text>();

        _objectiveManager = FindObjectsByType<ObjectiveManager>(FindObjectsSortMode.None).First();
        _objectiveManager.ObjectiveSelectedClientEvent += HandleObjectiveSelected;
    }

    void Update()
    {
        _countdownText.text = _selectedObjective != null
            ? _selectedObjective.PrepTimer.ToString("0.0")
            : string.Empty;
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
