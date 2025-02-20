using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ObjectiveTimer : MonoBehaviour
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

    void Update()
    {
        if (_selectedObjective == null)
        {
            _textComponent.text = "No Objective";
        }
        else
        {
            _textComponent.text = "Objective: " + _selectedObjective.ObjectiveLocationName;
        }
    }

    private void HandleObjectiveSelected(ulong selectedObjectiveNetworkObjectId)
    {
        _selectedObjective = FindObjectsByType<Objective>(FindObjectsSortMode.None)
            .First(o => o.NetworkObject.NetworkObjectId == selectedObjectiveNetworkObjectId);
    }
}
