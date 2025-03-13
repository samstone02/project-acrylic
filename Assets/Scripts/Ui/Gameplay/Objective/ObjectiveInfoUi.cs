using System.Linq;
using UnityEngine;

public class ObjectiveTimerUi : MonoBehaviour
{
    private GameObject ObjectiveLocationText;
    private GameObject ContestedIndicatorText;
    private GameObject CaptureDetails;
    private GameObject PrepDetails;

    private ObjectiveManager _objectiveManager;
    private Objective _selectedObjective;

    private void Awake()
    {
        ObjectiveLocationText = transform.Find("ObjectiveLocationText").gameObject;
        ContestedIndicatorText = transform.Find("ContestedIndicatorText").gameObject;
        CaptureDetails = GetComponentsInChildren<RectTransform>()
            .First(t => t.name == "CaptureDetails").gameObject;
        PrepDetails = GetComponentsInChildren<RectTransform>()
            .First(t => t.name == "PrepDetails").gameObject;

        _objectiveManager = FindObjectsByType<ObjectiveManager>(FindObjectsSortMode.None).First();
        _objectiveManager.ObjectiveSelectedClientEvent += HandleObjectiveSelected;
    }

    private void Update()
    {
        if (_selectedObjective == null)
        {
            ObjectiveLocationText.SetActive(false);
            ContestedIndicatorText.SetActive(false);
            CaptureDetails.SetActive(false);
            PrepDetails.SetActive(false);
        }
        else if (_selectedObjective.PrepTimer > 0)
        {
            ObjectiveLocationText.SetActive(true);
            PrepDetails.SetActive(true);
        }
        else
        {
            CaptureDetails.SetActive(true);
            PrepDetails.SetActive(false);
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