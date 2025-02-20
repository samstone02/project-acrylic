using System.Linq;
using TMPro;
using UnityEngine;

public class ObjectiveTimerUi : MonoBehaviour
{
    private ObjectiveManager _objectiveManager;
    private TMP_Text _textComponent;
    private Objective _selectedObjective;

    void Start()
    {
        _objectiveManager = FindObjectsByType<ObjectiveManager>(FindObjectsSortMode.None).First();
        _textComponent = GetComponentInChildren<TMP_Text>();

        _objectiveManager.ObjectiveSelectedClientEvent += HandleObjectiveSelected;
    }

    private void Update()
    {
        if (_selectedObjective == null)
        {
            _textComponent.text = "";
        }
        else if (_selectedObjective.PrepTimer > 0)
        {
            _textComponent.text = "Objective starting soon...";
        }
        else
        {
            _textComponent.text = _selectedObjective.CaptureTimer.ToString("0.00");
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
