using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControllingTeamUi : MonoBehaviour
{
    [field: SerializeField] public Vector3 ControllingOffset { get; private set; }
    [field: SerializeField] public Color ControllingColor { get; private set; }
    [field: SerializeField] private Color BlueControllingTextColor { get; set; }
    [field: SerializeField] private Color OrangeControllingTextColor { get; set; }

    private RectTransform BlueTeamBasePosition;
    private RectTransform OrangeTeamBasePosition;
    private RectTransform BlueTeamBox;
    private RectTransform OrangeTeamBox;

    private ObjectiveManager _objectiveManager;
    private Objective _selectedObjective;

    private Color NonControllingColor;

    void Start()
    {
        var componentsInChildern = GetComponentsInChildren<RectTransform>();

        BlueTeamBasePosition = componentsInChildern.First(r => r.name == "BlueTeamBasePosition");
        OrangeTeamBasePosition = componentsInChildern.First(r => r.name == "OrangeTeamBasePosition");
        BlueTeamBox = componentsInChildern.First(r => r.name == "BlueTeamBox");
        OrangeTeamBox = componentsInChildern.First(r => r.name == "OrangeTeamBox");

        _objectiveManager = FindAnyObjectByType<ObjectiveManager>();
        _objectiveManager.ObjectiveSelectedClientEvent += HandleObjectiveSelected;

        NonControllingColor = BlueTeamBox.GetComponent<Image>().color;
    }

    void Update()
    {
        if (_selectedObjective == null)
        {
            return;
        }

        if (_selectedObjective.IsBlueContesting)
        {
            BlueTeamBox.position = BlueTeamBasePosition.position + ControllingOffset;
            BlueTeamBox.GetComponent<Image>().color = ControllingColor;
            BlueTeamBox.GetComponentInChildren<TextMeshProUGUI>().color = BlueControllingTextColor;
        }
        else
        {
            BlueTeamBox.position = BlueTeamBasePosition.position;
            BlueTeamBox.GetComponent<Image>().color = NonControllingColor;
            BlueTeamBox.GetComponentInChildren<TextMeshProUGUI>().color = ControllingColor;
        }

        if (_selectedObjective.IsOrangeContesting)
        {
            OrangeTeamBox.position = OrangeTeamBasePosition.position - ControllingOffset;
            OrangeTeamBox.GetComponent<Image>().color = ControllingColor;
            OrangeTeamBox.GetComponentInChildren<TextMeshProUGUI>().color = OrangeControllingTextColor;
        }
        else
        {
            OrangeTeamBox.position = OrangeTeamBasePosition.position;
            OrangeTeamBox.GetComponent<Image>().color = NonControllingColor;
            OrangeTeamBox.GetComponentInChildren<TextMeshProUGUI>().color = ControllingColor;
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
